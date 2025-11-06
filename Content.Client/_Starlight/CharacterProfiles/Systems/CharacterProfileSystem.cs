// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.Medical.Cybernetics.Systems;
using Content.Client._Starlight.UI.Core;
using Content.Client.Humanoid;
using Content.Client.Lobby;
using Content.Shared._Starlight.CharacterProfiles;
using Content.Shared._Starlight.CharacterProfiles.Systems;
using Content.Shared.Clothing;
using Robust.Client.GameObjects;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterProfiles.Systems;

public sealed class CharacterProfileSystem : SharedCharacterProfileSystem
{
    [Dependency] private readonly UIEventBus _uiEventBus = default!;
    [Dependency] private readonly IClientPreferencesManager _preferences = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;
    [Dependency] private readonly CyberneticsSystem _cyberSystem = default!;
    [Dependency] private readonly LoadoutSystem _loadoutSystem = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly CharacterEditorSystem _characterEditor = default!;

    private CharacterProfileRegistry _characterRegistry = new();
    private Dictionary<int, Entity<SpriteComponent>> _profilePreviews = new();

    public bool SlotsFull => _characterRegistry.OccupiedSlots >= MaxCharacters;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeNetworkEvent<MsgSyncCharacterProfile>(HandleCharacterSync);
        SubscribeNetworkEvent<MsgDeleteCharacterProfile>(HandleCharacterDeleted);
    }

    private void HandleCharacterSync(MsgSyncCharacterProfile ev)
    {
        Entity<SpriteComponent> previewEnt;
        if (TryGetCharacterProfile(ev.Slot, out var existingProfile))
        {
            existingProfile.SetData(ev.Data);
            previewEnt = EnsurePreviewEntity(ev.Slot, existingProfile);
            ApplyToDoll(previewEnt, existingProfile);
            _uiEventBus.RaiseEvent(new CharacterSlotUpdatedUIEvent(ev.Slot,existingProfile, previewEnt));
            if (!_characterEditor.HasLiveProfile)
                _characterEditor.StartEditingSlot(ev.Slot);
            return;
        }
        if (ev.PartialData)
        {
            Log.Error($"Tried to create incomplete profile for slot:{ev.Slot}");
            return;
        }
        var newProfile = LoadExistingProfile(ev.Data);
        _characterRegistry.AddProfile(ev.Slot,newProfile);
        previewEnt = EnsurePreviewEntity(ev.Slot, newProfile);
        ApplyToDoll(previewEnt, newProfile);
        _uiEventBus.RaiseEvent(new CharacterSlotUpdatedUIEvent(ev.Slot, newProfile, previewEnt));
        if (!_characterEditor.HasLiveProfile)
            _characterEditor.StartEditingSlot(ev.Slot);
    }

    private void HandleCharacterDeleted(MsgDeleteCharacterProfile ev)
    {
        DeleteCharacterInSlot(ev.Slot, false);
    }

    public void ApplyProfileChanges(int slot, bool raiseOnServer = true)
    {
        if (!TryGetCharacterProfile(slot, out var profile))
        {
            Log.Error($"Could not apply changes for slot:{slot}, no profile found!");
            return;
        }

        if (!profile.HasDirtyData)
            return;
        if (raiseOnServer)
            RaiseNetworkEvent(new MsgUpdateCharacterProfile(slot, profile, true));
        profile.ClearDirty();
        RefreshPreviewVisuals(slot);
    }

    public void RefreshPreviewVisuals(int slot)
    {
        if (!TryGetCharacterProfile(slot, out var profile))
            return;
        var previewEntity = EnsurePreviewEntity(slot, profile);
        ApplyToDoll(previewEntity, profile);
    }

    public bool DeleteCharacterInSlot(int slot, bool raiseOnServer = true)
    {
        if (!_characterRegistry.DeleteProfile(slot, out var oldProfile))
            return false;
        ClearPreviewEntity(slot);
        if (raiseOnServer)
            RaiseNetworkEvent(new MsgDeleteCharacterProfile(slot));
        _uiEventBus.RaiseEvent(new CharacterSlotUpdatedUIEvent(slot,null, null));
        return true;
    }

    public bool CreateNewCharacter()
    {
        if (SlotsFull)
            return false;
        //We already check if slots are full, so this will always return a valid slot
        var availableSlot = _characterRegistry.GetFirstFreeSlot(MaxCharacters);
        return CreateNewCharacter(availableSlot);
    }

    public bool CreateNewCharacter(int slot)
    {
        CharacterProfile? profile;
        if (_characterRegistry.TryGetCharacterProfile(slot, out profile))
        {
            Log.Warning($"Tried to create character in slot:{slot} when one already exists!");
            return false;
        }
        profile = CreateRandomProfile();
        ValidateProfile(profile);
        RaiseNetworkEvent(new MsgUpdateCharacterProfile(slot, profile));
        _characterRegistry.AddProfile(slot, profile);
        var previewEnt = EnsurePreviewEntity(slot, profile);
        _uiEventBus.RaiseEvent(new CharacterSlotUpdatedUIEvent(slot,profile, previewEnt));
        return true;
    }

    public bool TryGetCharacterProfile(int slot, [NotNullWhen(true)] out CharacterProfile? profile)
    {
        return _characterRegistry.TryGetCharacterProfile(slot, out profile);
    }

    public Entity<SpriteComponent> EnsurePreviewEntity(int slot, CharacterProfile profile, CharacterPreviewMode previewMode = default)
    {
        if (_profilePreviews.TryGetValue(slot, out var previewEntity)) return previewEntity;
        var dollEnt = CreateProfileDoll(profile, previewMode);
        previewEntity = (dollEnt, Comp<SpriteComponent>(dollEnt));
        _profilePreviews[slot] = previewEntity;
        return previewEntity;
    }

    public bool TryGetPreviewEntity(int slot, out Entity<SpriteComponent> previewEntity)
    {
        return _profilePreviews.TryGetValue(slot, out previewEntity);
    }

    public bool ClearPreviewEntity(int slot)
    {
        if (!_profilePreviews.Remove(slot, out var ent))
            return false;
        EntityManager.DeleteEntity(ent);
        return true;
    }

    public CharacterProfile? GetFirstProfileOrNull()
    {
        return _characterRegistry.GetFirstProfileOrNull(MaxCharacters);
    }

    public int GetFirstProfileSlot()
    {
        return _characterRegistry.GetFirstProfileSlot(MaxCharacters);
    }
}