// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.Medical.Cybernetics.Systems;
using Content.Client.Humanoid;
using Content.Client.Lobby;
using Content.Shared._Starlight.CharacterProfileSystem;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.CharacterProfileSystem.Systems;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.UIEvents;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterProfiles.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class CharacterProfileSystem : SharedCharacterProfileSystem, IUIEventSubscriber
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IClientPreferencesManager _preferences = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    [Dependency] private readonly CyberneticsSystem _cyberSystem = default!;

    private Dictionary<int, (Entity<CharacterProfileComponent> Profile,
        Entity<SpriteComponent, HumanoidAppearanceComponent> Preview)> _slotToEntLookup = new();

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
        base.Initialize();
        SubscribeLocalEvent<CharacterProfileComponent, AfterAutoHandleStateEvent>(OnReceivedUpdatedCharacter);
    }

    public bool TryGetCharacterInSlot(int slot,
        out Entity<CharacterProfileComponent> profileEnt,
        out Entity<SpriteComponent, HumanoidAppearanceComponent> previewEnt)
    {

        if (!_slotToEntLookup.TryGetValue(slot, out var data))
        {
            profileEnt = default;
            previewEnt = default;
            return false;
        }
        profileEnt = data.Profile;
        previewEnt = data.Preview;
        return true;
    }

    private void OnReceivedUpdatedCharacter(Entity<CharacterProfileComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        Entity<SpriteComponent, HumanoidAppearanceComponent> previewSprite;
        if (!_slotToEntLookup.TryGetValue(ent.Comp.Slot, out var existing))
        {
            var newEnt = EntityManager.SpawnEntity(null, MapCoordinates.Nullspace);
            var spriteComp =  AddComp<SpriteComponent>(newEnt);
            var humanoidAppearance =  AddComp<HumanoidAppearanceComponent>(newEnt);
            previewSprite = (newEnt, spriteComp, humanoidAppearance);
            _slotToEntLookup[ent.Comp.Slot] = (ent, previewSprite);
        }
        else
        {
            previewSprite = existing.Preview;
        }
        UpdatePreviewEntity(previewSprite, ent);
        _uiManager.RaiseGlobalUIEvent( new CharacterProfileUpdatedUIEvent(ent));
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

    private void UpdatePreviewEntity(Entity<SpriteComponent, HumanoidAppearanceComponent> previewEntity, Entity<CharacterProfileComponent> profileEntity)
    {
        _humanoidSystem.LoadProfile(previewEntity, profileEntity.Comp.Data.Profile, previewEntity.Comp2);
        _cyberSystem.ApplyCyberneticVisuals((previewEntity, previewEntity.Comp2), profileEntity.Comp.Data.Profile);
    }
}