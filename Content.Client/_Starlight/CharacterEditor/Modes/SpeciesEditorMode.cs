// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Editors;
using Content.Client._Starlight.CharacterEditor.Editors.Species;

namespace Content.Client._Starlight.CharacterEditor.Modes;

public sealed class SpeciesEditorMode : CharacterEditorMode
{
    public override string ModeName => "Species";

    protected override void RegisterPanels()
    {
        RegisterPanel<SpeciesSelectorPanel>();
        RegisterPanel<IdentityPanel>();
    }
}