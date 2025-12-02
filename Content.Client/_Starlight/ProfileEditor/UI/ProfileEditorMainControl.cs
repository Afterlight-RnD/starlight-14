// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;


public interface IProfileEditorMainControl<TSelf, out TProfileEditor> : ISLControl
    where TSelf: SLControl, IProfileEditorMainControl<TSelf,TProfileEditor>
    where TProfileEditor: IProfileEditor<TProfileEditor>, new()
{
    public bool StepControlsInjected { get; set; }
    public TProfileEditor Editor { get; }
    public void AddStepButton(ProfileEditorStepButton button);
    public bool IsOpen { get; }
}
public abstract class ProfileEditorMainControl<TSelf, TProfileEditor, TPanelEnum> : SLControl,
    IProfileEditorMainControl<TSelf,TProfileEditor>
    where TSelf : ProfileEditorMainControl<TSelf,TProfileEditor, TPanelEnum>, new()
    where TProfileEditor : IProfileEditor<TProfileEditor, TSelf>, new()
    where TPanelEnum: struct, Enum, IConvertible

{
    public bool IsOpen => IsInsideTree;
    protected abstract Control StepSelectorRoot { get; }
    public bool StepControlsInjected { get; set; }
    public TProfileEditor Editor { get; private set; } = new();

    protected ProfileEditorMainControl()
    {
        Editor.Initialize((TSelf)this);
    }

    void IProfileEditorMainControl<TSelf,TProfileEditor>.AddStepButton(ProfileEditorStepButton button)
    {
        StepSelectorRoot.AddChild(button);
    }

    public abstract void InjectPanel(TPanelEnum panelEnum, SLControl panel);
}