// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem
{
    protected override void BoundControlEnteredTree(CharacterEditorControl boundControl)
    {
        base.BoundControlEnteredTree(boundControl);
        boundControl.OnEntered += OnEntered;
        boundControl.OnExited += OnExited;
        boundControl.OnSaveChanges += OnSaveChanges;
        boundControl.OnDiscardChanges += OnDiscardChanges;
    }

    protected override void BoundControlExitedTree(CharacterEditorControl boundControl)
    {
        base.BoundControlExitedTree(boundControl);
        boundControl.OnEntered -= OnEntered;
        boundControl.OnExited -= OnExited;
        boundControl.OnSaveChanges -= OnSaveChanges;
        boundControl.OnDiscardChanges -= OnDiscardChanges;
    }

    protected override void StepInitialized(CharacterEditorControl boundControl, CharacterEditorStep step)
    {
        boundControl.RegisterEditorStep(step);
    }
}