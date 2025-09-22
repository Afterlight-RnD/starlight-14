// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Server.Humanoid;
using Content.Server.Preferences.Managers;
using Content.Shared._Starlight.CharacterProfileSystem;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.CharacterProfileSystem.Systems;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Server.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Starlight.CharacterProfiles.Systems;

public sealed class CharacterProfileSystem : SharedCharacterProfileSystem
{
    [Dependency] private readonly PvsOverrideSystem _pvsOverride = default!;
    [Dependency] private readonly IServerPreferencesManager _preferences = default!;
    [Dependency] private readonly IDependencyCollection _dependencies = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;

    private Dictionary<NetUserId, Dictionary<int,Entity<CharacterProfileComponent>>> _profiles = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<CharacterProfileDataUpdateRequest>(HandleProfileUpdateRequest);
        SubscribeLocalEvent<PlayerPreferencesLoadedEvent>(OnPlayerPrefsLoaded);
        SubscribeLocalEvent<PlayerPreferencesUnloadedEvent>(OnPlayerPrefsUnloaded);
    }

    private void OnPlayerPrefsUnloaded(PlayerPreferencesUnloadedEvent ev)
    {
        if (!_profiles.Remove(ev.Session.UserId, out var profiles))
            return;
        foreach (var (_, profileEnt) in profiles)
            EntityManager.DeleteEntity(profileEnt);
    }

    private void OnPlayerPrefsLoaded(PlayerPreferencesLoadedEvent ev)
    {
        if (ev.Preferences == null)
            return;
        foreach (var (slot, profile) in ev.Preferences.Characters)
            LoadProfile(ev.Session,slot, new CharacterProfileData((HumanoidCharacterProfile)profile));
    }

    public void SetCharacterProfileData(ICommonSession owningSession, int slot, CharacterProfileData? newData)
    {
        if (MaxProfileSlots >= slot)
        {
            Log.Error($"UserId:{owningSession.UserId} tried to set profile data on slot out of range!");
            return;
        }

        var profileDict = _profiles[owningSession.UserId];

        if (newData == null)
        {
            if (profileDict.Remove(slot, out var removedProfile))
                EntityManager.DeleteEntity(removedProfile);
            _preferences.DeleteProfile(owningSession.UserId, slot);
            return;
        }

        var validate = new ValidateCharacterProfileEvent(newData);
        RaiseLocalEvent(ref validate);

        newData.Profile.EnsureValid(owningSession, _dependencies); //TODO: eventually replace with event-based validation
        if (!profileDict.TryGetValue(slot, out var profile))
        {
            profile = CreateNewProfileEntity(owningSession, slot, newData);
            profileDict.Add(slot, profile);
        }
        else profile.Comp.Data = newData;

        Dirty(profile);
        RaiseNetworkEvent(new ReceiveUpdatedCharacterProfileEvent(slot, GetNetEntity(profile)));

        //apply profile changes to the db
        _preferences.SetProfile(owningSession.UserId, slot, profile.Comp.Data.Profile);
    }

    private void HandleProfileUpdateRequest(CharacterProfileDataUpdateRequest msg, EntitySessionEventArgs args)
    {
        var profileEnt = GetEntity(msg.ProfileEnt);
        var profileComp = ProfileQuery.Comp(profileEnt);
        if (args.SenderSession.UserId != profileComp.OwnerNetId)
        {
            Log.Warning($"CharacterProfileComponent update owner-mismatch! Expected:{args.SenderSession.UserId} got: " +
                        $"{profileComp.OwnerNetId}");
            return;
        }
        SetCharacterProfileData(args.SenderSession, msg.Slot, msg.Data);
    }

    public Entity<CharacterProfileComponent> LoadProfile(ICommonSession session, int slot,
        CharacterProfileData profileData)
    {
        var validate = new ValidateCharacterProfileEvent(profileData);
        RaiseLocalEvent(ref validate);
        profileData.Profile.EnsureValid(session, _dependencies); //TODO: eventually replace with event-based validation
        return CreateNewProfileEntity(session, slot, profileData);
    }


    public Entity<CharacterProfileComponent> CreateNewProfileEntity(ICommonSession session, int slot,
        CharacterProfileData newData)
    {
        var newEnt = EntityManager.SpawnEntity(null, MapCoordinates.Nullspace);
        var newComp = new CharacterProfileComponent
        {
            Data = newData,
            OwnerNetId = session.UserId,
            Slot = slot
        };
        AddComp(newEnt,newComp);
        _humanoidSystem.LoadProfile(newEnt, newData.Profile);
        _pvsOverride.AddSessionOverride(newEnt, session);
        var profileEnt = new Entity<CharacterProfileComponent>(newEnt, newComp);
        Dirty(profileEnt);
        return profileEnt;
    }
}