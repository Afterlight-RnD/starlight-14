// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public sealed class CharacterEditorStepButton : ProfileEditorStepButton
{
    public override string StepLocPrefix => "character-editor-step";
    public override string StepDescriptionLocString => "character-editor-step-desc";
}