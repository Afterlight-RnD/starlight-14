// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles;
using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;
using Content.Shared._Starlight.CharacterProfiles.Data;
using Robust.Client.GameObjects;

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
        SubscribeUIEvent<ChangeCharacterEditorPreviewModeUIEvent>(HandlePreviewModeChange);
        SubscribeUIEvent<SelectCharacterProfileUIEvent>(HandleCharacterSelected);
        SubscribeUIRequest<EditCharacterProfileFieldUIRequest>(HandleEditProfileRequest);
    }

    private void HandleEditProfileRequest(ref EditCharacterProfileFieldUIRequest args)
    {
        args.Profile = _liveProfile;
    }

    private void HandleCharacterSelected(ref readonly SelectCharacterProfileUIEvent args)
    {
        if (_liveProfile != null)
        {
            if (_liveProfile.Slot == args.Slot)
                return;
            ClearPreviewEntity();
            _liveProfile = null;
        }
        StartEditingSlot(args.Slot);
    }

    private void HandlePreviewModeChange(ref readonly ChangeCharacterEditorPreviewModeUIEvent args)
    {
        ChangePreviewMode(args.NewMode);
    }

    public void ShutdownEditor()
    {
        ClearPreviewEntity();
        if (_liveProfile == null)
            return;

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
        }
        else ClearPreviewEntity();
        _liveProfile = new CharacterProfile(profile.GetData(false)) { Slot = slot };
        _livePreview = EnsureLivePreview(_previewmode);
        RefreshPreviewVisuals();
        RaiseUIEvent(new CharacterEditorPreviewChangedUIEvent(_livePreview.Value));
        RaiseUIEvent(new CharacterEditorProfileDirtiedUIEvent(_liveProfile));
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
        //RaiseUIEvent(new CharacterEditor(_liveProfile));
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
        //RaiseUIEvent(new CharacterEditingUpdateUIEvent(_liveProfile));
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
        RaiseUIEvent(new CharacterEditorPreviewChangedUIEvent(_livePreview.Value));
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

    public void SetLiveData<TProfileData, TValue>(TValue value,CharacterDataSetterDelegate<TProfileData, TValue> setter)
        where TProfileData : struct, ICharacterData
    {
        _liveProfile?.EditData(value, setter);
    }

    public void SetLiveData<TData>(TData data, bool dirty = true) where TData : struct, ICharacterData
    {
        _liveProfile?.SetData(data, dirty);
    }
}