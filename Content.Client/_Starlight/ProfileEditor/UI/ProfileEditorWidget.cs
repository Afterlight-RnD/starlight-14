// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;

public abstract class ProfileEditorWidget<TEditorControl> : SLWidget
    where TEditorControl: SLWidget, IProfileEditorControl, new()
{
    public TEditorControl? OwningEditor { get; private set; }

    protected void RaiseEditorUIEvent<TEvent>(TEvent args)
        where TEvent:struct
    {
        if (OwningEditor == null)
            throw new Exception("Cannot raise EditorEvent when OwningEditor is null!");
        UIEvents.RaiseControlEvent(OwningEditor, args);
    }

    protected override void EnteredTree()
    {
        OwningEditor = RecursivelyGetEditorParent(this);
        base.EnteredTree();
    }

    protected override void ExitedTree()
    {
        OwningEditor = null;
        base.ExitedTree();
    }

    private TEditorControl? RecursivelyGetEditorParent(Control? control)
    {
        switch (control)
        {
            case null:
                return null;
            case TEditorControl castControl:
                return castControl;
            default:
                return RecursivelyGetEditorParent(control.Parent);
        }
    }
}