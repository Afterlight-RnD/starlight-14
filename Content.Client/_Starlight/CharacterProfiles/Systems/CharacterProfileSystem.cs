// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.Medical.Cybernetics.Systems;
using Content.Client.Humanoid;
using Content.Client.Lobby;
using Content.Shared._Starlight.CharacterProfileSystem;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.CharacterProfileSystem.Systems;
using Content.Shared.Clothing;
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
    [Dependency] private readonly CyberneticsSystem _cyberSystem = default!;
    [Dependency] private readonly LoadoutSystem _loadoutSystem = default!;

    private Dictionary<int, Entity<CharacterProfileComponent>> _slotToProfile = new();
    private Dictionary<int, Entity<SpriteComponent, HumanoidAppearanceComponent>> _slotToPreview = new();

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

    private void OnReceivedUpdatedCharacter(Entity<CharacterProfileComponent> ent, ref AfterAutoHandleStateEvent ev)
    {
        _slotToProfile[ent.Comp.Slot] = ent;
        EnsurePreviewEntity(ent);
        _uiManager.RaiseGlobalUIEvent( new CharacterProfileUpdatedUIEvent(ent));
    }

    public bool TryGetCharacterInSlot(int slot,
        out Entity<CharacterProfileComponent> profileEnt,
        out Entity<SpriteComponent, HumanoidAppearanceComponent> previewEnt)
    {

        if (!_slotToProfile.TryGetValue(slot, out var profile))
        {
            profileEnt = default;
            previewEnt = default;
            return false;
        }
        profileEnt = profile;
        previewEnt = _slotToPreview[slot];
        return true;
    }

    private void EnsurePreviewEntity(Entity<CharacterProfileComponent> ent)
    {
        Entity<SpriteComponent, HumanoidAppearanceComponent> previewSprite;
        if (!_slotToPreview.TryGetValue(ent.Comp.Slot, out var existing))
        {
            var newEnt = EntityManager.SpawnEntity(null, MapCoordinates.Nullspace);
            var spriteComp =  AddComp<SpriteComponent>(newEnt);
            var humanoidAppearance =  AddComp<HumanoidAppearanceComponent>(newEnt);
            previewSprite = (newEnt, spriteComp, humanoidAppearance);
            _loadoutSystem.ApplyJobClothes(previewSprite, ent);
            _slotToPreview[ent.Comp.Slot] = previewSprite;
        }
        else
        {
            previewSprite = existing;
        }
        UpdatePreviewEntity(previewSprite, ent);
    }

    public void DirtyCharacter(Entity<CharacterProfileComponent> ent)
    {
        _uiManager.RaiseGlobalUIEvent(new CharacterProfileUpdatedUIEvent{CharacterProfile = new
            (ent.Owner, ent.Comp!)});
    }

    public void CreateNewCharacter()
    {
        _preferences.CreateCharacter(HumanoidCharacterProfile.Random());
    }

    public void SaveCharacterChanges(Entity<CharacterProfileComponent> target)
    {
        RaiseNetworkEvent(new MsgRequestCharacterProfileDataUpdate(
            GetNetEntity(target),
            GetOwningPlayerPrefsNet(target),
            target.Comp.Data, target.Comp.Slot));
    }

    private void UpdatePreviewEntity(Entity<SpriteComponent, HumanoidAppearanceComponent> previewEntity, Entity<CharacterProfileComponent> profileEntity)
    {
        _humanoidSystem.LoadProfile(previewEntity, profileEntity.Comp.Data.Profile, previewEntity.Comp2);
        _cyberSystem.ApplyCyberneticVisuals((previewEntity, previewEntity.Comp2), profileEntity.Comp.Data.Profile);
    }
}