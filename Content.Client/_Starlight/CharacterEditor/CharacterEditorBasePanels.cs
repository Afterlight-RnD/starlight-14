// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;

namespace Content.Client._Starlight.CharacterEditor;


public enum CharacterEditorPanelPosition
{
    Main,
    Right
}


public abstract class CharacterEditorBasePanel : ProfileEditorPanelBaseControl<CharacterEditor,
    CharacterEditorMainControl, CharacterEditorStep, CharacterEditorPanelPosition>
{
    protected override void InjectPanel(CharacterEditorMainControl editorControl)
    {
        switch (PanelPosition)
        {
            case CharacterEditorPanelPosition.Main:
                editorControl.MainPanel.AddChild(this);
                break;
            case CharacterEditorPanelPosition.Right:
                editorControl.SidePanel.AddChild(this);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

public abstract class CharacterEditorRightPanel : CharacterEditorBasePanel
{
    public override CharacterEditorPanelPosition PanelPosition => CharacterEditorPanelPosition.Right;
}

public abstract class CharacterEditorMainPanel : CharacterEditorBasePanel
{
    public override CharacterEditorPanelPosition PanelPosition => CharacterEditorPanelPosition.Main;
}