// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.UIEvents;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed class CharacterEditorSystem : UISystem
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;

    private CharacterProfile? _liveProfile = null;
    private Entity<SpriteComponent>? _livePreview = null;
    private CharacterPreviewMode _previewmode = default;

    public bool HasLiveProfile => _livePreview != null;
    public bool ProfileHasChanges => _liveProfile is { HasDirtyData: true };

    public override void Initialize()
    {
        SubscribeUIEvent<CharacterEditingHasChangesUIEvent>(OnProfileDirtied);
        SubscribeUIEvent<ProfileSelectorButton, ControlEnteredTreeUIEvent>(OnProfileButtonAdded);
        SubscribeUIEvent<ProfileSelectorButton, ControlExitedTreeUIEvent>(OnProfileButtonRemoved);
        SubscribeUIEvent<ProfileSelectorButton, ButtonPressedUIEvent>(OnProfileSelected);

        SubscribeUIEvent<CharacterPreviewModeButton, ButtonPressedUIEvent>(OnPreviewModeButtonPressed);
    }

    private void OnPreviewModeButtonPressed(CharacterPreviewModeButton control, ButtonPressedUIEvent ev)
    {
        ChangePreviewMode(control.PreviewMode);
    }

    private void OnProfileSelected(ProfileSelectorButton control, ButtonPressedUIEvent ev)
    {
        StartEditingSlot(control.Slot);
    }

    private void OnProfileButtonRemoved(ProfileSelectorButton control, ControlExitedTreeUIEvent ev)
    {
        control.SetFromProfile(null);
        control.SetPreviewSprite(null);
    }

    private void OnProfileButtonAdded(ProfileSelectorButton control, ControlEnteredTreeUIEvent ev)
    {
        _characterProfileSystem.TryGetCharacterProfile(control.Slot, out var profile);
        Entity<SpriteComponent>? previewEnt = null;
        if (profile != null)
            previewEnt = _characterProfileSystem.EnsurePreviewEntity(control.Slot, profile);
        control.SetFromProfile(profile);
        control.SetPreviewSprite(previewEnt);
    }

    private void OnProfileDirtied(CharacterEditingHasChangesUIEvent ev)
    {
        RefreshPreviewVisuals();
    }

    public void StartEditingSlot(int slot)
    {
        if (!_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
        {
            Log.Error($"Tried to start editing slot:{slot} but it doesn't have a profile!");
            return;
        }

        if (_liveProfile != null)
        {
            if (_liveProfile.Slot == slot)
                return;
            RaiseUIEvent(new CharacterEditingFinishedUIEvent(_liveProfile));
        }

        if (_livePreview != null)
        {
            EntityManager.DeleteEntity(_livePreview);
            _livePreview = null;
        }
        _liveProfile = new CharacterProfile(profile.GetData(false)) { Slot = slot };
        _livePreview = EnsureLivePreview();
        RaiseUIEvent(new CharacterEditingStartedUIEvent(_liveProfile, _livePreview.Value));
        RefreshPreviewVisuals();
    }

    public void ApplyChanges()
    {
        if (_liveProfile == null)
        {
            Log.Warning($"Tried to apply changes when no profile was being edited!");
            return;
        }
        if (!ProfileHasChanges)
            return;
        if (!_characterProfileSystem.TryGetCharacterProfile(_liveProfile.Slot, out var profile))
        {
            Log.Error($"Could not find profile for slot {_liveProfile.Slot} to apply changes!");
            return;
        }
        profile.SetData(_liveProfile.GetData());
        _characterProfileSystem.ApplyProfileChanges(_liveProfile.Slot);
        RaiseUIEvent(new CharacterEditingAppliedUIEvent(_liveProfile));
    }

    public void ChangePreviewMode(CharacterPreviewMode newPreviewMode)
    {
        if (_previewmode == newPreviewMode)
            return;
        _previewmode = newPreviewMode;
        RefreshPreviewVisuals();
    }

    private Entity<SpriteComponent> EnsureLivePreview()
    {
        if (_liveProfile == null)
            throw new InvalidOperationException("Cannot ensure preview without a live profile!");
        if (_livePreview != null)
            return _livePreview.Value;
        var dollEnt = _characterProfileSystem.CreateProfileDoll(_liveProfile);
        return (dollEnt, Comp<SpriteComponent>(dollEnt));
    }

    public void RefreshPreviewVisuals()
    {
        if (_liveProfile == null)
        {
            if (_livePreview != null)
                EntityManager.DeleteEntity(_livePreview);
            return;
        }
        _livePreview = EnsureLivePreview();
        _characterProfileSystem.ApplyToDoll(_livePreview.Value, _liveProfile, _previewmode);
    }
}