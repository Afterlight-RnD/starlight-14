// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Editors.Appearance;

namespace Content.Client._Starlight.CharacterEditor.Modes;

public sealed class AppearanceEditorMode : CharacterEditorMode
{
    //public override Type[] AfterModes => [typeof(SpeciesEditorMode)];
    public override string ModeName => "Appearance";

    protected override void RegisterPanels()
    {
        RegisterPanel<AppearancePanel>();
        RegisterPanel<MarkingsPanel>();
    }
}