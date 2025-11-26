// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.CharacterEditor.Controls;


public sealed class CharacterEditorPanelRoot : SLControl
{
    public CharacterEditorPanelLayout LayoutPosition { get; set; }= default;
}

public abstract partial class CharacterEditorPanel : SLBox
{
    public abstract CharacterEditorPanelLayout Layout { get; }

    public bool IsSide => Layout == CharacterEditorPanelLayout.Side;

    public bool IsMain => Layout == CharacterEditorPanelLayout.Main;
    protected CharacterEditorPanel()
    {
        Orientation = LayoutOrientation.Vertical;
        HorizontalExpand = true;
        VerticalExpand = true;
        HorizontalAlignment = HAlignment.Stretch;
        VerticalAlignment = VAlignment.Stretch;
        Margin = new(10);
    }
};

public abstract class MainCharacterEditorPanel : CharacterEditorPanel
{
    public override CharacterEditorPanelLayout Layout => CharacterEditorPanelLayout.Main;
}

public abstract class SideCharacterEditorPanel : CharacterEditorPanel
{
    public override CharacterEditorPanelLayout Layout => CharacterEditorPanelLayout.Side;
}

public enum CharacterEditorPanelLayout
{
    Main,
    Side
}