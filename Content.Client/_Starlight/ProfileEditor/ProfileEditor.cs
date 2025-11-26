// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;



public interface IProfileEditorControl : ISLControl
{
};

public interface IProfileEditor
{
    public bool SetStep(int stepIndex);
    public int CurrentStepIndex { get; }

    public Type StepType { get; }
    public Type EditorControlType { get; }

    public void RegisterStep(IProfileEditorStep editorSteps);

    public void CleanupSteps();

    public void Initialize(Control editorControl);

    public bool IsSetup { get; }
    public void FinishSetup();
}

public sealed class ProfileEditor<TEditorControl, TStep> : IProfileEditor
    where TEditorControl : Control, ISLControl, IProfileEditorControl
    where TStep : ProfileEditorStep<TEditorControl>
{
    [Dependency] private ILogManager _logManager = default!;

    public Type StepType => typeof(TStep);
    public Type EditorControlType => typeof(TEditorControl);

    public event Action<TStep>? OnStepExited;
    public event Action<TStep>? OnStepEntered;

    public TStep CurrentStep => _editorSteps[CurrentStepIndex];

    private TEditorControl _editorControl = default!;
    private List<TStep> _editorSteps = new();
    private ISawmill _log;

    public int CurrentStepIndex { get; private set; } = 0;

    public ProfileEditor()
    {
        _log = _logManager.GetSawmill($"ProfileEditor<{typeof(TEditorControl)}, {typeof(TStep)}>");
    }

    void IProfileEditor.RegisterStep(IProfileEditorStep editorStep)
    {
        var typedStep = (TStep)editorStep;
        _editorSteps.Add(typedStep);
        typedStep.InjectStepControls(_editorControl);
        OrderSteps();
    }

    void IProfileEditor.CleanupSteps()
    {
        foreach (var step in _editorSteps)
            step.RemoveStepControls(_editorControl);
        _editorSteps.Clear();
        IsSetup = false;
    }

    void IProfileEditor.Initialize(Control editorControl)
    {
        _editorControl = (TEditorControl)editorControl;
    }

    public bool IsSetup { get; private set; }

    void IProfileEditor.FinishSetup()
    {
        IsSetup = true;
        var curStep = CurrentStep;
        OnStepEntered?.Invoke(curStep);
        curStep.StepEntered(_editorControl);
    }

    public bool SetStep(int stepIndex)
    {
        if (!IsSetup)
        {
            _log.Error("Profile Editor not setup yet!");
            return false;
        }
        if (stepIndex >= _editorSteps.Count)
        {
            _log.Error($"Index:{stepIndex} is out of range of steps:{_editorSteps.Count}");
            return false;
        }
        if (stepIndex == CurrentStepIndex)
            return false;
        var currentStep = CurrentStep;
        OnStepExited?.Invoke(currentStep);
        currentStep.StepExited(_editorControl);
        var nextStep = _editorSteps[stepIndex];

        OnStepEntered?.Invoke(nextStep);
        nextStep.StepEntered(_editorControl);
        return true;
    }

    private void OrderSteps()
    {
        _editorSteps.Sort(((stepA, stepB) =>
        {
            var stepAType = stepA.GetType();
            var stepBType = stepB.GetType();

            if (stepA.BeforeSteps != null)
            {
                foreach (var before in stepA.BeforeSteps)
                {
                    if (before == stepBType)
                        return -1;
                }
            }
            if (stepA.AfterSteps != null)
            {
                foreach (var after in stepA.AfterSteps)
                {
                    if (after == stepBType)
                        return 1;
                }
            }

            if (stepB.BeforeSteps != null)
            {
                foreach (var before in stepB.BeforeSteps)
                {
                    if (before == stepAType)
                        return 1;
                }
            }
            if (stepB.AfterSteps != null)
            {
                foreach (var after in stepB.AfterSteps)
                {
                    if (after == stepAType)
                        return -1;
                }
            }
            return 0;
        }));
    }
}
