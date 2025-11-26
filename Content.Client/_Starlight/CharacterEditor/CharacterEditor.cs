// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor;
using Content.Client._Starlight.UI;
using Robust.Client.Graphics;

namespace Content.Client._Starlight.CharacterEditor;

public sealed class CharacterEditorStep(
    string modeName,
    CharacterEditorPanel? mainPanel,
    CharacterEditorPanel? sidePanel,
    string? modeSubText = null,
    Texture? modeIcon = null)
    : ProfileEditorStep<CharacterEditorControl>
{
    public Texture? ModeIcon => modeIcon;

    public string ModeName => modeName;

    public string? ModeSubText => modeSubText;

    private CharacterEditorPanel? MainPanel { get; init; } = mainPanel;
    private CharacterEditorPanel? SidePanel{ get; init; } = sidePanel;


    public override void InjectControls(CharacterEditorControl editorControl)
    {
        if (MainPanel != null)
        {
            MainPanel.Visible = false;
            editorControl.MainPanel.AddChild(MainPanel);
        }
        if (SidePanel != null)
        {
            SidePanel.Visible = false;
            editorControl.SidePanel.AddChild(SidePanel);
        }
    }

    public override void Activated(CharacterEditorControl editorControl)
    {
        if (MainPanel != null)
            MainPanel.Visible = true;
        if (SidePanel != null)
            SidePanel.Visible = true;
    }

    public override void Deactivated(CharacterEditorControl editorControl)
    {
        if (MainPanel != null)
            MainPanel.Visible = false;
        if (SidePanel != null)
            SidePanel.Visible = false;
    }
}

public abstract class CharacterEditorPanel : SLBox
{
    public CharacterEditorPanel()
    {
        Orientation = LayoutOrientation.Vertical;
        HorizontalExpand = true;
        VerticalExpand = true;
    }
}
