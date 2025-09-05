// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Numerics;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.CharacterEditor.Widgets;

public abstract class EditorModeWidget : UIWidget
{
    public Action<string?>? OnSecondaryLabelUpdated = null;
    public EditorModeWidget()
    {
        Orientation = LayoutOrientation.Vertical;
        Margin = new Thickness(5);

        HorizontalAlignment = HAlignment.Stretch;
        VerticalAlignment = VAlignment.Stretch;
        HorizontalExpand = true;
        VerticalExpand = true;
    }
}