// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem
{
    [Dependency] private Dictionary<Type, ICharacterEditorView> _editorViews = new();


    private void RegisterEditorModes()
    {

    }
}