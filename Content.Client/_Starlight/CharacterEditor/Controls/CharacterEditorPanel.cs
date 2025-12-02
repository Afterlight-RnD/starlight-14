// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;

namespace Content.Client._Starlight.CharacterEditor.Controls;


public abstract partial class CharacterEditorPanel : SLBox
{

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