// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.Medical.Cybernetics.Systems;
using Content.Client.Humanoid;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.UIEvents;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public enum CharacterPreviewMode
{
    Loadout,
    Nude
}

public sealed class CharacterEditorSystem : UISystem
{
    [Dependency] private readonly IUserInterfaceManager _uiMan = default!;
    [Dependency] private readonly CharacterProfileSystem _profileSystem = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;
    [Dependency] private readonly CyberneticsSystem _cybernetics = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;


    private Entity<CharacterProfileComponent, HumanoidAppearanceComponent, SpriteComponent>? _liveProfile = null;
    private bool _liveProfileDirty = false;

    private int _liveSlot = -1;
    public int LiveSlot => _liveSlot;

    public CharacterPreviewMode PreviewMode { get; private set; } = default;
    public Entity<CharacterProfileComponent, HumanoidAppearanceComponent, SpriteComponent> LiveProfile
    {
        get
        {
            if (_liveProfile.HasValue) return _liveProfile.Value;
            var ent = EntityManager.Spawn(null, MapCoordinates.Nullspace);
            var profileComp = AddComp<CharacterProfileComponent>(ent);
            var humanoidComp = AddComp<HumanoidAppearanceComponent>(ent);
            var spriteComp = AddComp<SpriteComponent>(ent);
            _liveProfile = (ent, profileComp, humanoidComp,spriteComp);
            return _liveProfile.Value;
        }
        set
        {
            _liveProfile = value;
        }
    }

    public override void Initialize()
    {
        SubscribeUIEvent<ProfileSelectorButton, ButtonPressedUIEvent>(OnProfileSelectorPressed);
        SubscribeUIEvent<ProfileSelectorButton, ControlEnteredTreeUIEvent>(OnProfileSelectorAdded);
    }

    private void OnProfileSelectorPressed(ProfileSelectorButton control, ButtonPressedUIEvent ev)
    {
        if (control.Profile == null)
        {
            Log.Error($"Selected a slot with a null profile!");
            return;
        }

        if (_liveProfile != null && control.Slot == _liveProfile.Value.Comp1.Slot)
            return;
        LoadProfileAsLive(control.Profile.Value);
    }

    private void OnProfileSelectorAdded(ProfileSelectorButton control, ControlEnteredTreeUIEvent ev)
    {
        if (_profileSystem.TryGetCharacterInSlot(control.Slot, out var profileEnt, out _))
        {
            control.SetFromProfile(profileEnt, _protoManager.Index(profileEnt.Comp.FavoriteJob));
        }
        else
        {
            control.SetFromProfile(null, null);
        }
    }

    public void MarkLiveProfileAsDirty()
    {
        if (_liveProfileDirty)
            return;
        _liveProfileDirty = true;
        _uiMan.RaiseUIEvent(new LiveCharacterProfileDirtiedUIEvent(LiveProfile));
    }

    public void SaveLiveCharacterChanges()
    {
        if (!_liveProfileDirty)
            return;
        _profileSystem.SaveCharacterChanges(LiveProfile);
    }

    public void ChangePreviewMode(CharacterPreviewMode newMode)
    {
        if (newMode == PreviewMode)
            return;
        PreviewMode = newMode;
        //TODO: update clothing
        var faveJob = LiveProfile.Comp1.FavoriteJob;
    }

    public void LoadProfileAsLive(Entity<CharacterProfileComponent> profile)
    {
        var liveProfile = LiveProfile;
        CopyComp(profile, liveProfile, profile.Comp);

        _humanoidSystem.LoadProfile(liveProfile, profile.Comp.Data.Profile, liveProfile.Comp2);
        _cybernetics.ApplyCyberneticVisuals((liveProfile, liveProfile.Comp2), profile.Comp.Data.Profile);
        RaiseUIEvent(new LiveCharacterProfileUpdatedUIEvent(liveProfile));
    }

    public bool ApplyLiveProfile()
    {
        if (_liveSlot < 0
            || !_liveProfile.HasValue
            || !_profileSystem.TryGetCharacterInSlot(_liveSlot, out var profileEnt, out var dollEnt))
            return false;

        CopyComp(_liveProfile.Value.Owner, profileEnt, _liveProfile.Value.Comp1);

        _humanoidSystem.LoadProfile(dollEnt, profileEnt.Comp.Data.Profile, dollEnt.Comp2);
        _cybernetics.ApplyCyberneticVisuals((profileEnt, dollEnt.Comp2), profileEnt.Comp.Data.Profile);
        _profileSystem.DirtyCharacter((profileEnt, profileEnt.Comp));
        return true;
    }

    public void ClearLiveProfile()
    {
        _liveSlot = -1;
        if (!_liveProfile.HasValue)
            return;
        EntityManager.DeleteEntity(_liveProfile);
    }
}