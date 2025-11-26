// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;

public interface IProfileEditorStep
{
    public void InjectControls(Control root);

    public void Enter(Control root);

    public void Exit(Control root);
}


public abstract class ProfileEditorStep<TEditorControl> : IProfileEditorStep
    where TEditorControl: Control, ISLControl, IProfileEditorControl
{
    public abstract void InjectControls(TEditorControl editorControl);

    public abstract void Activated(TEditorControl editorControl);
    public abstract void Deactivated(TEditorControl editorControl);

    void IProfileEditorStep.InjectControls(Control root)
    {
        InjectControls((TEditorControl)root);
    }

    void IProfileEditorStep.Enter(Control root)
    {
        Activated((TEditorControl)root);
    }

    void IProfileEditorStep.Exit(Control root)
    {
        Deactivated((TEditorControl)root);
    }

}