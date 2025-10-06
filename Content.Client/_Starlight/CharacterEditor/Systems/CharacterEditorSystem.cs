// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.Medical.Cybernetics.Systems;
using Content.Client.Humanoid;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Content.Shared.Station;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.UIEvents;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public enum CharacterPreviewMode
{
    Loadout,
    Nude
}

public sealed class CharacterEditorSystem : EntitySystem, IUIEventSubscriber
{
    [Dependency] private readonly IUserInterfaceManager _uiMan = default!;
    [Dependency] private readonly CharacterProfileSystem _profileSystem = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;
    [Dependency] private readonly CyberneticsSystem _cybernetics = default!;


    private Entity<CharacterProfileComponent, HumanoidAppearanceComponent, SpriteComponent>? _liveProfile = null;

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
        _uiMan.SubscribeGlobalUIEvent<CharacterProfileSelectedUIEvent>(this, OnProfileSelected);
        _uiMan.SubscribeUIEvent<ProfileSelectorButton, ControlAddedUIEvent>(this, OnProfileSelectorAdded);
    }

    private void OnProfileSelectorAdded(ProfileSelectorButton control, ControlAddedUIEvent ev)
    {
        if (_profileSystem.TryGetCharacterInSlot(control.Slot, out var profileEnt, out _))
        {
            control.SetFromProfile(profileEnt);
        }
        else
        {
            control.SetFromProfile(null);
        }
    }

    private void OnProfileSelected(CharacterProfileSelectedUIEvent ev)
    {
        CreateLiveProfile(ev.Slot);
    }

    public void ChangePreviewMode(CharacterPreviewMode newMode)
    {
        if (newMode == PreviewMode)
            return;
        PreviewMode = newMode;
        //TODO: update clothing
        var JobPref = LiveProfile.Comp1.Data.Profile.JobPreferences.First();
        _uiMan.RaiseGlobalUIEvent(new LiveCharacterPreviewModeUpdatedUIEvent(PreviewMode));
    }



    public void CreateLiveProfile(int slot)
    {
        if (_liveSlot == slot || !_profileSystem.TryGetCharacterInSlot(slot, out var profileEnt,
                out _))
            return;
        var profile = LiveProfile;
        profile.Comp1.Slot = slot;
        profile.Comp1.Data = profileEnt.Comp.Data;
        _humanoidSystem.LoadProfile(profile, profileEnt.Comp.Data.Profile, profile.Comp2);
        _cybernetics.ApplyCyberneticVisuals((profile, profile.Comp2), profileEnt.Comp.Data.Profile);
        _uiMan.RaiseGlobalUIEvent(new LiveCharacterProfileUpdatedUIEvent(slot, LiveProfile));
    }

    public bool ApplyLiveProfile()
    {
        if (_liveSlot < 0
            || !_liveProfile.HasValue
            || !_profileSystem.TryGetCharacterInSlot(_liveSlot, out var profileEnt, out _))
            return false;

        profileEnt.Comp.Data = _liveProfile.Value.Comp1.Data;
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