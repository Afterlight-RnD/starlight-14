// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem
{
    protected override void BoundControlEnteredTree(CharacterEditorControl boundControl)
    {
        boundControl._editorSystem = this;
        base.BoundControlEnteredTree(boundControl);
    }

    protected override void BoundControlExitedTree(CharacterEditorControl boundControl)
    {
        base.BoundControlExitedTree(boundControl);
        boundControl._editorSystem = null;
    }

    protected override void StepInitialized(CharacterEditorControl boundControl, CharacterEditorStep step)
    {
        boundControl.RegisterEditorStep(step);
    }
}