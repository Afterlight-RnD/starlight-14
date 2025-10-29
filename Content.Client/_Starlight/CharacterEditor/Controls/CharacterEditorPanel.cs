// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public sealed class CharacterEditorPanelStub : Control
{
    public CharacterEditorPanelLayout EditorLayout { get; private set; } = default;

    public CharacterEditorPanelStub()
    {
        HorizontalExpand = true;
        VerticalExpand = true;
        HorizontalAlignment = HAlignment.Stretch;
        VerticalAlignment = VAlignment.Stretch;
    }
}

public enum CharacterEditorPanelLayout
{
    Main,
    Side,
}