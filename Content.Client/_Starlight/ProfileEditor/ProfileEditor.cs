// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;



public interface IProfileEditorControl : ISLControl
{
    public bool EditorControlsInjected { get; set; }

    public bool RequireExitConfirmation { get; set; }
};

public sealed class ProfileEditor<TEditorControl, TStep>
    where TEditorControl : Control, ISLControl, IProfileEditorControl
    where TStep : ProfileEditorStep<TEditorControl>
{
    public int CurrentStep { get; private set; } = -1;

    public int StepCount => _steps.Count;

    private readonly List<TStep> _steps = new();

    public IEnumerable<TStep> IterateSteps()
    {
        foreach (var step in _steps)
            yield return step;
    }

    public TStep GetCurrentStep => _steps[CurrentStep];

    public TStep GetStep(int stepIndex)
    {
        return _steps[stepIndex];
    }

    public bool SetStep(int step, TEditorControl editorControl)
    {
        if (step >= StepCount || step < 0)
            throw new InvalidOperationException("Tried to set step out of range!");
        if (CurrentStep == step)
            return false;
        if (CurrentStep >= 0)
        {
            GetCurrentStep.Deactivated(editorControl);
        }

        CurrentStep = step;
        GetCurrentStep.Activated(editorControl);
        return true;
    }

    public void InjectControls(TEditorControl editorControl)
    {
        foreach (var step in _steps)
            step.InjectControls(editorControl);
    }

    public void RegisterStep(IProfileEditorStep step)
    {
        _steps.Add((TStep)step);
    }
}