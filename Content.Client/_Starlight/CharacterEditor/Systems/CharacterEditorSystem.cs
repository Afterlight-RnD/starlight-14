// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.ProfileEditor;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem : ProfileEditorSystem<CharacterEditorMainControl, CharacterEditorStep>
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;

    private void OnEntered(CharacterEditorMainControl editorMain)
    {
    }

    private void OnExited(CharacterEditorMainControl editorMain)
    {
    }

    private void OnDiscardChanges(CharacterEditorMainControl boundMainControl, bool clearProfile)
    {
    }

    private void OnSaveChanges(CharacterEditorMainControl boundMainControl)
    {
    }

    public Entity<SpriteComponent> EnsurePreviewEntity(CharacterEditorMainControl editorMainControl)
    {
        if (editorMainControl.LiveProfile == null)
            throw new InvalidOperationException("Cannot ensure preview without a live profile!");
        if (editorMainControl.PreviewEntity != null)
            return editorMainControl.PreviewEntity.Value;
        var dollEnt = _characterProfileSystem.CreateProfileDoll(editorMainControl.LiveProfile, editorMainControl.PreviewMode);
        editorMainControl.PreviewEntity = (dollEnt, Comp<SpriteComponent>(dollEnt));
        // RaiseUIEvent(new CharacterEditorPreviewChangedUIEvent(editorControl.PreviewEntity.Value));
        return editorMainControl.PreviewEntity.Value;
    }

    public void EnsureValidProfileSlot(CharacterEditorMainControl editorMainControl)
    {
        if (editorMainControl.CurrentSlot == -1
            || !_characterProfileSystem.TryGetCharacterProfile(editorMainControl.CurrentSlot, out _))
            editorMainControl.CurrentSlot = _characterProfileSystem.GetFirstProfileSlot();
    }

    public void StartEditingSlot(CharacterEditorMainControl editorMainControl, int slot)
    {
        if (!_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
        {
            Log.Error($"Tried to start editing slot:{slot} but it doesn't have a profile!");
            return;
        }

        if (editorMainControl.LiveProfile != null)
        {
            if (editorMainControl.LiveProfile.Slot == slot)
                return;
        }
        else ClearPreviewEntity(editorMainControl);

        editorMainControl.LiveProfile = new CharacterProfile(profile.GetData(false)) { Slot = slot };
        editorMainControl.PreviewEntity = EnsurePreviewEntity(editorMainControl);
        RefreshPreviewVisuals(editorMainControl);
    }

    public void ApplyEdits(CharacterEditorMainControl editorMainControl, int slot)
    {
        if (editorMainControl.LiveProfile == null || !_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
        {
            Log.Error($"Tried to apply  slot:{slot} edits but it doesn't have a profile!");
            return;
        }
        profile.SetData(editorMainControl.LiveProfile.GetData());
        _characterProfileSystem.ApplyProfileChanges(slot);
    }

    public void RefreshPreviewVisuals(CharacterEditorMainControl editorMainControl)
    {
        if (editorMainControl.LiveProfile == null)
            ClearPreviewEntity(editorMainControl);

        editorMainControl.PreviewEntity = EnsurePreviewEntity(editorMainControl);
    }

    public void ClearPreviewEntity(CharacterEditorMainControl editorMainControl)
    {
        if (editorMainControl.PreviewEntity != null)
        {
            EntityManager.DeleteEntity(editorMainControl.PreviewEntity);
            editorMainControl.PreviewEntity = null;
        }
    }
}