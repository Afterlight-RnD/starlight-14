// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.UI;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.ProfileEditor.UI;

public interface IProfileEditorMainControl<TEditor>
    where TEditor: IProfileEditor, new()
{
}



public abstract class ProfileEditorMainControl<TEditor, TLayoutEnum, TStepSelectorButton> : SLControl, IProfileEditorMainControl<TEditor>
where TEditor: IProfileEditor, new()
where TLayoutEnum: struct, Enum, IConvertible
where TStepSelectorButton: ProfileEditorStepButton, new()
{

    public abstract Control StepButtonRoot { get; }
    public TEditor Editor { get;  init; } = default!;
    public Control?[,] StepControls { private get; init; } = default!; //this *should* always be set after the control is created
    private ButtonGroup _stepSelectorButtonGroup = new(false);
    public void ToggleStepControls(int step, bool state)
    {
        if (step >= Editor.StepCount)
            throw new IndexOutOfRangeException($"Tried to set step:{step} on {GetType()}, Max is: {Editor.StepCount-1}");
        for (var i = 0; i < Editor.LayoutCount; i++)
        {
            var control = StepControls[step, i];
            if (control != null)
                control.Visible = state;
        }
    }

    public void TryInjectStepControls(int step,
        string stepName,
        string? stepDescription,
        Texture? stepIcon,
        IDynamicTypeFactory typeFactory,
        Dictionary<TLayoutEnum, Func<IDynamicTypeFactory,Control>> builders)
    {
        StepButtonRoot.AddChild(new TStepSelectorButton
        {
            Group = _stepSelectorButtonGroup,
            Label = stepName,
            Description = stepDescription,
            Icon = stepIcon
        });
        foreach (var (layout,builder) in builders)
        {
            var layoutId = layout.ToInt32(null);
            var existing = StepControls[step,  layoutId];
            if (existing != null)
                continue;
            var newControl = builder.Invoke(typeFactory);
            newControl.Visible = false;
            InjectPanel(layout, newControl);
            StepControls[step, layoutId] = newControl;
        }
    }

    protected abstract void InjectPanel(TLayoutEnum layout, Control newControl);

    public TPanel GetPanel<TPanel>(int step, TLayoutEnum layout)
    where TPanel: Control, new()
    {
        var panel = StepControls[step, layout.ToInt32(null)];
        if (panel == null)
            throw new KeyNotFoundException($"Panel of type{typeof(TPanel)} not found in step:{step} pos: {layout}");
        return (TPanel)panel;
    }
}