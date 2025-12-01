// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.ProfileEditor;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem : ProfileEditorSystem<CharacterEditorControl, CharacterEditorStep>
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;

    private void OnEntered(CharacterEditorControl editor)
    {
    }

    private void OnExited(CharacterEditorControl editor)
    {
    }

    private void OnDiscardChanges(CharacterEditorControl boundControl, bool clearProfile)
    {
    }

    private void OnSaveChanges(CharacterEditorControl boundControl)
    {
    }

    public Entity<SpriteComponent> EnsurePreviewEntity(CharacterEditorControl editorControl)
    {
        if (editorControl.LiveProfile == null)
            throw new InvalidOperationException("Cannot ensure preview without a live profile!");
        if (editorControl.PreviewEntity != null)
            return editorControl.PreviewEntity.Value;
        var dollEnt = _characterProfileSystem.CreateProfileDoll(editorControl.LiveProfile, editorControl.PreviewMode);
        editorControl.PreviewEntity = (dollEnt, Comp<SpriteComponent>(dollEnt));
        // RaiseUIEvent(new CharacterEditorPreviewChangedUIEvent(editorControl.PreviewEntity.Value));
        return editorControl.PreviewEntity.Value;
    }

    public void EnsureValidProfileSlot(CharacterEditorControl editorControl)
    {
        if (editorControl.CurrentSlot == -1
            || !_characterProfileSystem.TryGetCharacterProfile(editorControl.CurrentSlot, out _))
            editorControl.CurrentSlot = _characterProfileSystem.GetFirstProfileSlot();
    }

    public void StartEditingSlot(CharacterEditorControl editorControl, int slot)
    {
        if (!_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
        {
            Log.Error($"Tried to start editing slot:{slot} but it doesn't have a profile!");
            return;
        }

        if (editorControl.LiveProfile != null)
        {
            if (editorControl.LiveProfile.Slot == slot)
                return;
        }
        else ClearPreviewEntity(editorControl);

        editorControl.LiveProfile = new CharacterProfile(profile.GetData(false)) { Slot = slot };
        editorControl.PreviewEntity = EnsurePreviewEntity(editorControl);
        RefreshPreviewVisuals(editorControl);
    }

    public void ApplyEdits(CharacterEditorControl editorControl, int slot)
    {
        if (editorControl.LiveProfile == null || !_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
        {
            Log.Error($"Tried to apply  slot:{slot} edits but it doesn't have a profile!");
            return;
        }
        profile.SetData(editorControl.LiveProfile.GetData());
        _characterProfileSystem.ApplyProfileChanges(slot);
    }

    public void RefreshPreviewVisuals(CharacterEditorControl editorControl)
    {
        if (editorControl.LiveProfile == null)
            ClearPreviewEntity(editorControl);

        editorControl.PreviewEntity = EnsurePreviewEntity(editorControl);
    }

    public void ClearPreviewEntity(CharacterEditorControl editorControl)
    {
        if (editorControl.PreviewEntity != null)
        {
            EntityManager.DeleteEntity(editorControl.PreviewEntity);
            editorControl.PreviewEntity = null;
        }
    }
}