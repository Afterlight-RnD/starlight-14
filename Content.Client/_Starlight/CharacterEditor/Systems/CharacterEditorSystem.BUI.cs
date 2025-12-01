// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem
{
    protected override void BoundControlEnteredTree(CharacterEditorMainControl boundMainControl)
    {
        base.BoundControlEnteredTree(boundMainControl);
        boundMainControl.OnEntered += OnEntered;
        boundMainControl.OnExited += OnExited;
        boundMainControl.OnSaveChanges += OnSaveChanges;
        boundMainControl.OnDiscardChanges += OnDiscardChanges;
    }

    protected override void BoundControlExitedTree(CharacterEditorMainControl boundMainControl)
    {
        base.BoundControlExitedTree(boundMainControl);
        boundMainControl.OnEntered -= OnEntered;
        boundMainControl.OnExited -= OnExited;
        boundMainControl.OnSaveChanges -= OnSaveChanges;
        boundMainControl.OnDiscardChanges -= OnDiscardChanges;
    }

    protected override void StepInitialized(CharacterEditorMainControl boundMainControl, CharacterEditorStep step)
    {
        boundMainControl.RegisterEditorStep(step);
    }
}