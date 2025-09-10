// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Numerics;
using Content.Client._Starlight.CharacterEditor.Controls;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.CharacterEditor.Widgets;

public abstract class EditorModeWidget : UIWidget
{
    private List<EditorModeButton> _linkedButtons = new();
    public EditorModeWidget()
    {
        Orientation = LayoutOrientation.Vertical;
        Margin = new Thickness(5);

        HorizontalAlignment = HAlignment.Stretch;
        VerticalAlignment = VAlignment.Stretch;
        HorizontalExpand = true;
        VerticalExpand = true;
    }

    public IEnumerable<EditorModeButton> EnumerateButtons()
    {
        foreach (var button in _linkedButtons)
        {
            yield return button;
        }
    }

    public void LinkButton(EditorModeButton newButton)
    {
        if (_linkedButtons.Contains(newButton))
            return;
        _linkedButtons.Add(newButton);
    }

    public void UnlinkButton(EditorModeButton newButton)
    {
        _linkedButtons.Remove(newButton);
    }
}