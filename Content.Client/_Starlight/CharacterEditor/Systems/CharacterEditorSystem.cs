// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.UIEvents;
using Robust.Shared.Map;

namespace Content.Client._Starlight.CharacterEditor.Systems;
public sealed class CharacterEditorSystem : EntitySystem, IUIEventSubscriber
{
    [Dependency] private readonly IUserInterfaceManager _uiMan = default!;
    [Dependency] private readonly CharacterProfileSystem _profileSystem = default!;

    private (Entity<CharacterProfileComponent> Profile,
        Entity<SpriteComponent, HumanoidAppearanceComponent> Preview)? _activeProfile = null;
    public override void Initialize()
    {
        _uiMan.SubscribeGlobalUIEvent<CharacterProfileSelectedUIEvent>(this, OnSlotSelected);
        _uiMan.SubscribeUIEvent<ProfileSelectorButton, ControlAddedUIEvent>(this, OnProfileSelectorAdded);
    }

    private void OnProfileSelectorAdded(ProfileSelectorButton control, ControlAddedUIEvent ev)
    {
        if (_profileSystem.TryGetCharacterInSlot(control.Slot, out var profileEnt, out _))
        {
            control.UpdateCharacterProfile(profileEnt);
        }
        else
        {
            control.UpdateCharacterProfile(null);
        }
    }

    private void OnSlotSelected(CharacterProfileSelectedUIEvent ev)
    {
        if (!_profileSystem.TryGetCharacterInSlot(ev.Slot,
                out var newProfile,
                out var newPreview))
        {
            Log.Error($"Tried to activate invalid slot!");
            return;
        }
        if (_activeProfile.HasValue)
        {
            if (_activeProfile.Value.Profile.Comp.Slot == ev.Slot)
            {
                Log.Warning($"Tried to select profile in slot:{ev.Slot} that was already active!");
                return;
            }
            CopyComp(newProfile, _activeProfile.Value.Profile, newProfile.Comp);
            CopyComps(newPreview, _activeProfile.Value.Preview, null,
                newPreview.Comp1, newPreview.Comp2);
        }
        else
        {
            var newProfileEnt = Spawn(null, MapCoordinates.Nullspace);
            var profileComp = AddComp<CharacterProfileComponent>(newProfileEnt);
            var newPreviewEnt = Spawn(null, MapCoordinates.Nullspace);
            var spriteComp = AddComp<SpriteComponent>(newPreviewEnt);
            var previewComp = AddComp<HumanoidAppearanceComponent>(newPreviewEnt);
            _activeProfile =
                new (
                    (newProfileEnt, profileComp),(newPreviewEnt, spriteComp, previewComp));
        }

        CopyComp(newProfile, _activeProfile.Value.Profile, newProfile.Comp);
        CopyComps(newPreview, _activeProfile.Value.Preview, null,
            newPreview.Comp1, newPreview.Comp2);
        _uiMan.RaiseGlobalUIEvent(new ActiveCharacterProfileUpdatedUIEvent(ev.Slot, _activeProfile.Value.Profile, _activeProfile.Value.Preview) );
    }
}