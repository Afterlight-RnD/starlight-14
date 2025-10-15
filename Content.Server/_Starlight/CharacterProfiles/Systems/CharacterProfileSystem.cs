// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Content.Server.Preferences.Managers;
using Content.Shared._Starlight.CharacterProfiles;
using Content.Shared._Starlight.CharacterProfiles.Systems;
using Content.Shared.Preferences;
using Robust.Shared.Network;

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
        SubscribeNetworkEvent<MsgCreateCharacterProfile>(HandleCreateCharacter);
    }

    private void HandleCreateCharacter(MsgCreateCharacterProfile msg, EntitySessionEventArgs args)
    {
        if (!TryGetCharacterRegistry(args.SenderSession.UserId, out var registry))
            return;
        CharacterProfile? newProfile;
        if (!registry.TryGetCharacterProfile(msg.Slot, out newProfile))
            newProfile = CreateRandomProfile();
        RaiseNetworkEvent(new MsgLoadCharacterProfile(msg.Slot, newProfile.GetData(false)), args.SenderSession);
    }

    private void HandleDeleteCharacter(MsgDeleteCharacterProfile msg, EntitySessionEventArgs args)
    {
        //TODO: Raise DB delete
        if (!_characterProfiles.TryGetValue(args.SenderSession.UserId, out var registry))
            return;
        registry.DeleteProfile(msg.Slot);
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

    private void HandleCharacterUpdate(MsgUpdateCharacterProfile msg, EntitySessionEventArgs args)
    {
        if (!TryGetCharacterRegistry(args.SenderSession.UserId, out var registry)
            || !TryGetCharacterInRegistry(registry, msg.Slot, args.SenderSession.UserId, out var profile))
            return;
        profile.SetFromList(msg.Data);
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


    private void OnPlayerPrefsLoaded(PlayerPreferencesLoadedEvent ev)
    {
        var registry = EnsureRegistry(ev.Session.UserId);

        foreach (var (slot, profile) in ev.Preferences.Characters)
        {
            if (profile is not HumanoidCharacterProfile legacyProfile)
            {
                Log.Warning($"profile is of unsupported type:{profile.GetType()}");
                continue;
            }
            var newProfile = CreateProfile();
            ConvertLegacyProfile(newProfile, legacyProfile);
            registry.SetProfile(slot, newProfile);
            ClearDirty(newProfile);
            RaiseNetworkEvent(new MsgLoadCharacterProfile(slot, newProfile.GetData(false)), ev.Session);
        }
    }

}