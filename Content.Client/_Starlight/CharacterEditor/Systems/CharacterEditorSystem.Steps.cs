// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Editors;
using Content.Client._Starlight.CharacterEditor.Editors.Appearance;
using Content.Client._Starlight.CharacterEditor.Editors.Species;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem
{
    protected override void DefineSteps()
    {
        RegisterStep(new CharacterEditorStep("Identity",new IdentityPanel(), null));
        RegisterStep(new CharacterEditorStep("Species",null, new SpeciesSelectorPanel()));
        RegisterStep(new CharacterEditorStep("Appearance",new AppearancePanel(), new MarkingsPanel()));
    }
}