// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.UI.Core;
namespace Content.Client._Starlight.CharacterEditor.Systems;

public interface ICharacterEditorView
{
    public void Setup(CharacterEditorPanelRoot panelRoot, CharacterEditorPanel panelInstance);

    public void EnterView(CharacterEditorPanel panelInstance);

    public void ExitView(CharacterEditorPanel panelInstance);
};

public abstract class CharacterEditorView<TEditorPanel> : UISystem, ICharacterEditorView
    where TEditorPanel: CharacterEditorPanel, new()
{
    public virtual void SetupMode(TEditorPanel panelInstance){}

    public virtual void ViewEntered(TEditorPanel panel){}

    public virtual void ViewExited(TEditorPanel panel){}

    public void Setup(CharacterEditorPanelRoot panelRoot, CharacterEditorPanel panelInstance)
    {
        if (panelInstance.Parent != null)
        {
            Log.Error($"{panelInstance}: already has a parent!");
            return;
        }

        if (panelRoot.LayoutPosition != panelInstance.Layout)
        {
            Log.Error($"{panelInstance} layout is: {panelInstance.Layout} expected: {panelRoot.LayoutPosition}");
            return;
        }
        panelInstance.Visible = false;
        panelRoot.AddChild(panelInstance);
    }

    public void EnterView(CharacterEditorPanel panelInstance)
    {
        panelInstance.Visible = true;
        ViewEntered((TEditorPanel)panelInstance);
    }

    public void ExitView(CharacterEditorPanel panelInstance)
    {
        ViewExited((TEditorPanel)panelInstance);
        panelInstance.Visible = false;
    }
}