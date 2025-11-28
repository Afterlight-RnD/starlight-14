// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Content.Server.Preferences.Managers;
using Content.Shared._Starlight.CharacterProfiles;
using Content.Shared._Starlight.CharacterProfiles.Data;
using Content.Shared._Starlight.CharacterProfiles.Systems;
using Content.Shared.Preferences;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._Starlight.CharacterProfiles.Systems;

public sealed class CharacterProfileSystem : SharedCharacterProfileSystem
{
    [Dependency] private readonly IServerPreferencesManager _preferencesManager = default!;

    private Dictionary<NetUserId, CharacterProfileRegistry> _characterProfiles = new();

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<PlayerPreferencesLoadedEvent>(OnPlayerPrefsLoaded);
        SubscribeNetworkEvent<MsgUpdateCharacterProfile>(HandleCharacterUpdate);
        SubscribeNetworkEvent<MsgDeleteCharacterProfile>(HandleDeleteCharacter);
    }

    protected override void HandleProtoReloaded(PrototypesReloadedEventArgs args)
    {
        var changeSet = new HashSet<Type>(args.Modified);
        changeSet.IntersectWith(ProtoReloadEvents.Keys);

        foreach (var (_, registry) in _characterProfiles)
            foreach (var profile in registry.IterateProfiles())
                RaiseProtoReloadOnProfile(changeSet, profile);
    }

    private void HandleDeleteCharacter(MsgDeleteCharacterProfile msg, EntitySessionEventArgs args)
    {
        //Don't raise on the client if we received a delete request to prevent recursive loops
        DeleteCharacter(args.SenderSession, msg.Slot, false);
    }

    private void HandleCharacterUpdate(MsgUpdateCharacterProfile msg, EntitySessionEventArgs args)
    {
        if (!TryGetCharacterRegistry(args.SenderSession.UserId, out var registry))
            return;
        if (!registry.TryGetCharacterProfile(msg.Slot, out var existing))
        {
            existing = CreateProfile();
        }
        existing.SetData(msg.Data);
        _preferencesManager.SLSaveCharacter(args.SenderSession, msg.Slot, existing);
    }

    public bool DeleteCharacter(ICommonSession userSession, int slot, bool raiseOnClient = true)
    {
        if (!_characterProfiles.TryGetValue(userSession.UserId, out var registry) || !registry.DeleteProfile(slot))
            return false;
        if (raiseOnClient)
            RaiseNetworkEvent(new MsgDeleteCharacterProfile(slot), userSession);
        //TODO: DB delete
        _preferencesManager.SLDeleteCharacter(userSession, slot);
        return true;
    }

    private bool TryGetCharacterRegistry(NetUserId userId, [NotNullWhen(true)] out CharacterProfileRegistry? registry)
    {
        if (_characterProfiles.TryGetValue(userId, out registry)) return true;
        Log.Warning($"Could not find character registry for user:{userId}, or it is not loaded yet!");
        return false;
    }

    private bool TryGetCharacterInRegistry(CharacterProfileRegistry registry,
        int slot, NetUserId userId,
        [NotNullWhen(true)] out CharacterProfile? profile)
    {
        if (registry.TryGetCharacterProfile(slot, out profile)) return true;
        Log.Warning($"Could not find character registry for user:{userId}, or it is not loaded yet!");
        return false;
    }


    public bool TryGetCharacterProfile(NetUserId userId, int slot, [NotNullWhen(true)] out CharacterProfile? profile)
    {
        profile = null;
        return TryGetCharacterRegistry(userId, out var registry) && registry.TryGetCharacterProfile(slot ,out profile);
    }

    private CharacterProfileRegistry EnsureRegistry(NetUserId userId)
    {
        if (_characterProfiles.TryGetValue(userId, out var existing))
            return existing;
        var newRegistry = new CharacterProfileRegistry();
        _characterProfiles.Add(userId,newRegistry);
        return newRegistry;
    }

    public CharacterProfile LoadProfile(ICommonSession userSession, int slot, List<CharacterData> characterData)
    {
        var registry = EnsureRegistry(userSession.UserId);
        var newProfile = LoadExistingProfile(characterData);
        registry.AddProfile(newProfile);
        RaiseNetworkEvent(new MsgSyncCharacterProfile(slot, newProfile), userSession);
        return newProfile;
    }


    private void OnPlayerPrefsLoaded(PlayerPreferencesLoadedEvent ev)
    {
        foreach (var (slot, profile) in ev.Preferences.Characters)
        {
            if (profile is not HumanoidCharacterProfile legacyProfile)
            {
                Log.Warning($"profile is of unsupported type:{profile.GetType()}");
                continue;
            }
            //TODO: Legacy conversion
            LoadProfile(ev.Session, slot,
            [
                new LegacyCharacterData { LegacyProfile = legacyProfile},
                new CharacterIdentityData(),
                new CharacterRoleData(), new CharacterSpeciesData()
            ]);
        }
    }

}