// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
namespace Content.Client._Starlight.CharacterEditor.Controls;

public abstract class CharacterEditorPanel : SLBox
{
    public CharacterEditorPanelLayout Layout => CharacterEditorPanelLayout.Main;

    public bool IsSide => Layout == CharacterEditorPanelLayout.Side;

    public bool IsMain => Layout == CharacterEditorPanelLayout.Main;
    protected CharacterEditorPanel()
    {
        HorizontalExpand = true;
        VerticalExpand = true;
        HorizontalAlignment = HAlignment.Stretch;
        VerticalAlignment = VAlignment.Stretch;
    }
};

public enum CharacterEditorPanelLayout
{
    Main,
    Side
}