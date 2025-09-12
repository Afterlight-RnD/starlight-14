// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Server.Preferences.Managers;
using Content.Shared._Starlight.CharacterProfileSystem;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.CharacterProfileSystem.Systems;
using Content.Shared.Preferences;
using Robust.Server.GameStates;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server._Starlight.CharacterProfiles.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class CharacterProfileSystem : SharedCharacterProfileSystem
{
    [Dependency] private readonly PvsOverrideSystem _pvsOverride = default!;
    [Dependency] private readonly IServerPreferencesManager _preferences = default!;

    public override void Initialize()
    {
        SubscribeLocalEvent<CharacterProfileComponent, ComponentGetStateAttemptEvent>(OnAttemptGetState);
        SubscribeLocalEvent<PlayerPreferencesLoadedEvent>(OnPrefsLoadedForSession);
        SubscribeLocalEvent<PlayerPreferencesUnloadedEvent>(OnPrefsUnloadedForSession);
        SubscribeNetworkEvent<CharacterProfileDataUpdateRequest>(HandleProfileUpdateRequest);
    }

    private void HandleProfileUpdateRequest(CharacterProfileDataUpdateRequest msg, EntitySessionEventArgs args)
    {
        var profileEnt = GetEntity(msg.ProfileEnt);
        var profileComp = ProfileQuery.Comp(profileEnt);
        if (args.SenderSession.UserId != profileComp.OwnerNetId)
        {
            Log.Warning($"CharacterProfileComponent update owner-mismatch! Expected:{args.SenderSession.UserId} got: {profileComp.OwnerNetId}");
            return;
        }
        var validate = new ValidateCharacterProfileEvent(msg.Data);
        if (msg.Data.Profile == null)
        {
            Log.Warning($"{args.SenderSession.UserId} tried to sync null character profile!");
            return;
        }
        RaiseLocalEvent(ref validate);
        if (validate.InvalidData)
        {
            Log.Warning($"Character Profile owned by: {args.SenderSession.UserId} in slot: {msg.Slot} failed validation!");
            Dirty(profileEnt, profileComp); //resync the profile with the valid server data,
                                            //this should only ever happen if the client bypasses local validation

            //TODO: possibly raise a network event to display an error message to the user without resetting to match the server data?
            return;
        }

        if (msg.Data == profileComp.Data)
            return;
        _preferences.SetProfile(args.SenderSession.UserId, msg.Slot, msg.Data.Profile);
        //profileComp.Data = msg.Data;
        //TODO Need to update comp data in this roundabout way because existing validation code is done in SetProfile
        profileComp.Data =
            new CharacterProfileData
            {
                Profile = (HumanoidCharacterProfile)_preferences.GetPreferences(args.SenderSession.UserId).GetProfile(msg.Slot)
            };
        Dirty(profileEnt, profileComp);

        var updatedEv = new CharacterProfileUpdatedEvent((profileEnt, profileComp));
        RaiseLocalEvent(ref updatedEv);
    }

    private Entity<CharacterProfileComponent> CreateNewProfile(ICommonSession session, int slot,
        HumanoidCharacterProfile characterProfile)
    {
        var newEnt = EntityManager.SpawnEntity(null, MapCoordinates.Nullspace);
        var newComp = new CharacterProfileComponent
        {
            OwnerNetId = session.UserId,
            Data = new CharacterProfileData{Profile = characterProfile},
            Slot = slot
        };
        AddComp(newEnt,newComp);
        _pvsOverride.AddSessionOverride(newEnt, session);
        return (newEnt, newComp);
    }

    private void OnPrefsUnloadedForSession(ref PlayerPreferencesUnloadedEvent ev)
    {
        var enumerator = EntityQueryEnumerator<CharacterProfileComponent>();
        while (enumerator.MoveNext(out var ent,out var comp))
        {
            if (comp.OwnerNetId == ev.Session.UserId)
                EntityManager.QueueDeleteEntity(ent);
        }

    }

    private void OnPrefsLoadedForSession(ref PlayerPreferencesLoadedEvent ev)
    {
        if (ev.Preferences == null)
            throw new Exception("TEST");
        foreach (var (slot, character) in ev.Preferences.Characters)
        {
            if (character is not HumanoidCharacterProfile humanoidCharacter)
                throw new Exception("Only HumanoidCharacterProfiles are supported!");
            CreateNewProfile(ev.Session, slot, humanoidCharacter);
            return;
        }
    }

    private void OnAttemptGetState(Entity<CharacterProfileComponent> ent, ref ComponentGetStateAttemptEvent args)
    {
        args.Cancelled = ent.Comp.OwnerNetId != args.Player?.UserId;
    }
}