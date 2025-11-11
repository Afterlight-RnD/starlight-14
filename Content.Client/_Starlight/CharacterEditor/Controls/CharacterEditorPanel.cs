// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public abstract partial class CharacterEditorPanel : SLBox
{
    public abstract CharacterEditorPanelLayout Layout { get; }

    public bool IsSide => Layout == CharacterEditorPanelLayout.Side;

    public bool IsMain => Layout == CharacterEditorPanelLayout.Main;

    private bool _allowFieldRegistrations = true;
    protected CharacterEditorPanel()
    {
        Orientation = LayoutOrientation.Vertical;
        HorizontalExpand = true;
        VerticalExpand = true;
        HorizontalAlignment = HAlignment.Stretch;
        VerticalAlignment = VAlignment.Stretch;
        Margin = new(10);
    }
    protected override void EnteredTree()
    {
        base.EnteredTree();
        _allowFieldRegistrations = false;
    }

    private void FieldRegsAllowed(Control newField)
    {
        if (!_allowFieldRegistrations)
            throw new InvalidOperationException(
                $"Tried to create EditorField:{newField} outside of constructor!");
    }

};

public enum CharacterEditorPanelLayout
{
    Main,
    Side
}