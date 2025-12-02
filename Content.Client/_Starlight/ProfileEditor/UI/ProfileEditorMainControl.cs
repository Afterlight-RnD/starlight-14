// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;


public interface IProfileEditorMainControl<TSelf, out TProfileEditor> : ISLControl
    where TSelf: SLControl, IProfileEditorMainControl<TSelf,TProfileEditor>
    where TProfileEditor: IProfileEditor<TProfileEditor>, new()
{
    public bool HasProfileChanges { get; }
    public bool StepControlsInjected { get; set; }
    public TProfileEditor Editor { get; }
    public void AddStepButton(ProfileEditorStepButton button);
    public bool IsOpen { get; }

    public void SetStepCount(int stepCount);
}
public abstract class ProfileEditorMainControl<TSelf, TProfileEditor, TPanelEnum> : SLControl,
    IProfileEditorMainControl<TSelf,TProfileEditor>
    where TSelf : ProfileEditorMainControl<TSelf,TProfileEditor, TPanelEnum>, new()
    where TProfileEditor : IProfileEditor<TProfileEditor, TSelf>, new()
    where TPanelEnum: struct, Enum, IConvertible

{
    public bool IsOpen => IsInsideTree;

    void IProfileEditorMainControl<TSelf, TProfileEditor>.SetStepCount(int stepCount)
    {
        if (_stepPanels.Count > 0)
            throw new Exception("Cannot Set StepCount on editor that already has steps!");
        for (var i = 0; i < stepCount; i++)
            _stepPanels.Add(new HashSet<Control>());
    }

    protected abstract Control StepSelectorRoot { get; }
    public bool HasProfileChanges => Editor.HasProfileChanges;
    public bool StepControlsInjected { get; set; }
    public TProfileEditor Editor { get; private set; } = new();

    private Dictionary<(TPanelEnum layout,Type type), Control> _panelLookup = new();

    private List<HashSet<Control>> _stepPanels = new();

    protected ProfileEditorMainControl()
    {
        Editor.Initialize((TSelf)this);
        Editor.OnStepEntered += HandleStepEntered;
        Editor.OnStepExited += HandleStepExited;
    }

    private void HandleStepExited(TProfileEditor _, int step)
    {
        foreach (var panel in _stepPanels[step])
            panel.Visible = false;
    }

    private void HandleStepEntered(TProfileEditor _, int step)
    {
        foreach (var panel in _stepPanels[step])
            panel.Visible = true;
    }

    void IProfileEditorMainControl<TSelf,TProfileEditor>.AddStepButton(ProfileEditorStepButton button)
    {
        StepSelectorRoot.AddChild(button);
    }

    public TPanel GetPanel<TPanel>(TPanelEnum panelEnum)
    where TPanel: Control, new()
    {
        return (TPanel)_panelLookup[(panelEnum,typeof(TPanel))];
    }

    public void RegisterPanel(int step,TPanelEnum panelEnum, Control panel)
    {
        if (!_stepPanels[step].Add(panel)
            || !_panelLookup.TryAdd((panelEnum, panel.GetType()), panel))
            return;
        InjectPanel(panelEnum, panel);
    }

    protected abstract void InjectPanel(TPanelEnum panelEnum, Control panel);
}