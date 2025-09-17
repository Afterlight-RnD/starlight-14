// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client.Lobby;
using Content.Shared._Starlight.CharacterProfileSystem;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.CharacterProfileSystem.Systems;
using Content.Shared.Preferences;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.CharacterProfiles.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class CharacterProfileSystem : SharedCharacterProfileSystem
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IClientPreferencesManager _preferences = default!;

    private Dictionary<int, Entity<CharacterProfileComponent>> _slotToEntLookup = new();

    public bool SlotsFull
    {
        get
        {
            //if pref data is not loaded, we can't check slots, so we assume full
            if (_preferences.Preferences == null || _preferences.Settings == null)
                return true;
            return _preferences.Preferences.Characters.Count >= _preferences.Settings.MaxCharacterSlots;
        }
    }
    public override void Initialize()
    {
        SubscribeLocalEvent<CharacterProfileComponent, AfterAutoHandleStateEvent>(OnReceivedUpdatedCharacter);
    }

    public bool TryGetCharacterInSlot(int slot, out Entity<CharacterProfileComponent> entity)
    {
        return _slotToEntLookup.TryGetValue(slot, out entity);
    }

    private void OnReceivedUpdatedCharacter(Entity<CharacterProfileComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        _slotToEntLookup.TryAdd(ent.Comp.Slot, ent);
        var ev = new CharacterProfileUpdatedEvent(ent);
        RaiseLocalEvent(ref ev);
        _uiManager.RaiseGlobalUIEvent(new CharacterProfileUpdatedUIEvent(ent));
    }

    public void DirtyCharacter(Entity<CharacterProfileComponent?> ent)
    {
        if (!ProfileQuery.Resolve(ref ent))
            throw new ArgumentException($"{ToPrettyString(ent)} must have a CharacterProfileComponent!");
        _uiManager.RaiseGlobalUIEvent(new CharacterProfileUpdatedUIEvent{CharacterProfile = new(ent.Owner, ent.Comp!)});
    }

    public void CreateNewCharacter()
    {
        _preferences.CreateCharacter(HumanoidCharacterProfile.Random());
    }

    public void SaveCharacterChanges(Entity<CharacterProfileComponent> target)
    {
        RaiseNetworkEvent(new CharacterProfileDataUpdateRequest(GetNetEntity(target), target.Comp.Data, target.Comp.Slot));
    }

    private void EnsurePreviewEntity(Entity<CharacterProfileComponent> target)
    {

    }
}