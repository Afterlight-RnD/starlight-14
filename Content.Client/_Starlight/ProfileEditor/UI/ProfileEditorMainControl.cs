// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.UI;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.ProfileEditor.UI;

public interface IProfileEditorMainControl<TEditor>
    where TEditor: IProfileEditor, new()
{
}

public abstract class ProfileEditorMainControl<TEditor, TProfile, TLayoutEnum, TStepSelectorButton> : SLControl, IProfileEditorMainControl<TEditor>
where TEditor: IProfileEditor<TProfile>, new()
where TProfile: class, IPersistentProfile
where TLayoutEnum: struct, Enum, IConvertible
where TStepSelectorButton: ProfileEditorStepButton, new()
{

    public abstract Control StepSelectorRoot { get; }
    public TEditor Editor { get;  init; } = default!;

    private Control?[,] _controls = default!;
    public Control?[,] StepControls { private get => _controls; init => _controls = value; } //this *should* always be set after the control is created
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
        IProfileEditorPanelInjector<TProfile, TLayoutEnum> builder)
    {
        StepSelectorRoot.AddChild(new TStepSelectorButton
        {
            Group = _stepSelectorButtonGroup,
            Label = stepName,
            Description = stepDescription,
            Icon = stepIcon
        });
        builder.InjectPanels(Editor.Profile, step, ref _controls, InjectPanel);
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