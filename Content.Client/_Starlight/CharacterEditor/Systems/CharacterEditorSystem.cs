// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.UIEvents;
using Robust.Shared.Reflection;

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
        SubscribeUIEvent<ValueDropdown<CharacterPreviewMode>.Option, ButtonPressedUIEvent>(OnPreviewModeButtonPressed);
    }

    private void OnPreviewModeButtonPressed(ValueDropdown<CharacterPreviewMode>.Option control, ButtonPressedUIEvent ev)
    {
        ChangePreviewMode(control.Value);
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
        if (_liveProfile == null && profile != null)
            StartEditingSlot(control.Slot);
    }

    private void OnProfileDirtied(CharacterEditingHasChangesUIEvent ev)
    {
        RefreshPreviewVisuals();
    }
    public void ShutdownEditor()
    {
        ClearPreviewEntity();
        if (_liveProfile == null)
            return;
        RaiseUIEvent(new CharacterEditingFinishedUIEvent(_liveProfile));
        _liveProfile = null;
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
            if (!_liveProfile.HasDirtyData)
            {
                RaiseUIEvent(new CharacterEditingFinishedUIEvent(_liveProfile));
                ClearPreviewEntity();
            }
        }
        else ClearPreviewEntity();
        _liveProfile = new CharacterProfile(profile.GetData(false)) { Slot = slot };
        _livePreview = EnsureLivePreview(_previewmode);
        RefreshPreviewVisuals();
        RaiseUIEvent(new CharacterEditingStartedUIEvent(_liveProfile, _livePreview.Value));
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


    public void DiscardChanges()
    {
        if (_liveProfile == null)
            return;
        if (!_characterProfileSystem.TryGetCharacterProfile(_liveProfile.Slot, out var profile))
        {
            _liveProfile = null;
            return;
        }
        _liveProfile.SetData(profile.GetData());
        ClearPreviewEntity();
        RefreshPreviewVisuals();
        RaiseUIEvent(new CharacterEditingUpdateUIEvent(_liveProfile));
    }

    public void ChangePreviewMode(CharacterPreviewMode newPreviewMode)
    {
        if (_previewmode == newPreviewMode)
            return;
        _previewmode = newPreviewMode;
        ClearPreviewEntity();
        RefreshPreviewVisuals();
    }

    private Entity<SpriteComponent> EnsureLivePreview(CharacterPreviewMode previewMode = default)
    {
        if (_liveProfile == null)
            throw new InvalidOperationException("Cannot ensure preview without a live profile!");
        if (_livePreview != null)
            return _livePreview.Value;
        var dollEnt = _characterProfileSystem.CreateProfileDoll(_liveProfile, previewMode);
        _livePreview = (dollEnt, Comp<SpriteComponent>(dollEnt));
        RaiseUIEvent(new CharacterEditingUpdatedPreviewEntityUIEvent(_livePreview.Value));
        return _livePreview.Value;
    }

    public void RefreshPreviewVisuals()
    {
        if (_liveProfile == null)
           ClearPreviewEntity();

        _livePreview = EnsureLivePreview(_previewmode);
    }

    public void ClearPreviewEntity()
    {
        if (_livePreview != null)
        {
            EntityManager.DeleteEntity(_livePreview);
            _livePreview = null;
        }
    }
}