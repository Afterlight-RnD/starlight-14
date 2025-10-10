// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Server.Preferences.Managers;
using Content.Shared._Starlight.CharacterProfileSystem;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.CharacterProfileSystem.Systems;
using Content.Shared._Starlight.Preferences.Components;
using Robust.Server.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Player;

namespace Content.Server._Starlight.CharacterProfiles.Systems;

public sealed class CharacterProfileSystem : SharedCharacterProfileSystem
{
    [Dependency] private readonly PvsOverrideSystem _pvsOverride = default!;
    [Dependency] private readonly IServerPreferencesManager _preferences = default!;
    [Dependency] private readonly IDependencyCollection _dependencies = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<MsgRequestCharacterProfileDataUpdate>(HandleProfileUpdateRequest);
    }

    public void SetCharacterProfileData(ICommonSession owningSession, Entity<PlayerPreferencesComponent> playerPrefs,
        int slot, CharacterProfileData? newData)
    {
        if (MaxProfileSlots >= slot)
        {
            Log.Error($"UserId:{owningSession.UserId} tried to set profile data on slot out of range!");
            return;
        }

        if (newData == null)
        {
            if (playerPrefs.Comp.CharacterProfiles.Remove(slot, out var removedProfile))
                EntityManager.DeleteEntity(removedProfile);
            _preferences.DeleteProfile(owningSession.UserId, slot); //TODO: legacy prefs
            return;
        }

        var validate = new ValidateCharacterProfileEvent(newData);
        RaiseLocalEvent(ref validate);

        newData.Profile.EnsureValid(owningSession, _dependencies); //TODO: eventually replace with event-based validation

        var profile = playerPrefs.Comp.CharacterProfiles.TryGetValue(slot, out var profileEnt)
            ? (profileEnt, ProfileQuery.Comp(profileEnt))
            : CreateNewProfileEntity(owningSession, slot, playerPrefs, newData);

        Dirty(profile);
        RaiseNetworkEvent(new MsgReceiveUpdatedCharacterProfile(slot, GetNetEntity(profile)));

        //apply profile changes to the db
        _preferences.SetProfile(owningSession.UserId, slot, profile.Comp.Data.Profile);
    }

    private void HandleProfileUpdateRequest(MsgRequestCharacterProfileDataUpdate msg, EntitySessionEventArgs args)
    {
        var profileEnt = GetEntity(msg.ProfileEnt);
        var preferencesEnt = GetEntity(msg.PreferencesEnt);
        var profileComp = ProfileQuery.Comp(profileEnt);
        if (args.SenderSession.UserId != profileComp.OwnerNetId)
        {
            Log.Warning($"CharacterProfileComponent update owner-mismatch! Expected:{args.SenderSession.UserId} got: " +
                        $"{profileComp.OwnerNetId}");
            return;
        }
        SetCharacterProfileData(args.SenderSession, (preferencesEnt, PlayerPrefQuery.Comp(preferencesEnt)),msg.Slot, msg.Data);
    }

    public Entity<CharacterProfileComponent> LoadProfile(ICommonSession session,
        Entity<PlayerPreferencesComponent> playerPrefs,
        int slot,
        CharacterProfileData profileData)
    {
        var validate = new ValidateCharacterProfileEvent(profileData);
        RaiseLocalEvent(ref validate);
        profileData.Profile.EnsureValid(session, _dependencies); //TODO: eventually replace with event-based validation
        return CreateNewProfileEntity(session, slot, playerPrefs, profileData);
    }


    private Entity<CharacterProfileComponent> CreateNewProfileEntity(
        ICommonSession session, int slot,
        Entity<PlayerPreferencesComponent> playerPrefs,
        CharacterProfileData newData)
    {
        var newEnt = EntityManager.SpawnEntity(null, MapCoordinates.Nullspace);
        var newComp = new CharacterProfileComponent
        {
            Data = newData,
            EnabledJobs = [..newData.Profile.JobPreferences],
            JobLoadouts = new(newData.Profile.Loadouts.Count),
            OwnerNetId = session.UserId,
            FavoriteJob = FallbackJob,
            Slot = slot
        };
        foreach (var (key, loadout) in newData.Profile.Loadouts)
            newComp.JobLoadouts.Add(key, loadout);
        AddComp(newEnt,newComp);
        var profileEnt = new Entity<CharacterProfileComponent>(newEnt, newComp);
        playerPrefs.Comp.CharacterProfiles.Add(slot, profileEnt);
        TransformSystem.SetParent(profileEnt, playerPrefs);
        Dirty(profileEnt);
        return profileEnt;
    }
}