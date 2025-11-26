// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;

public abstract class ProfileEditorSystem<TEditorControl, TStep> : BoundUISystem<TEditorControl>
    where TEditorControl : Control, ISLControl, IProfileEditorControl, new()
    where TStep : ProfileEditorStep<TEditorControl>
{
    private bool _stepLock = false;

    private ProfileEditor<TEditorControl, TStep> _editor = new();

    public override void Initialize()
    {
        base.Initialize();
        DefineSteps();
        _stepLock = true;
    }

    protected abstract void DefineSteps();

    protected void RegisterStep(TStep newStep)
    {
        if (_stepLock)
            throw new InvalidOperationException("Steps must be registered inside DefineSteps()");
        _editor.RegisterStep(newStep);
    }

    protected override void BoundControlEnteredTree(TEditorControl boundControl)
    {
        if (!boundControl.EditorControlsInjected)
        {
            _editor.InjectControls(boundControl);
            foreach (var step in  _editor.IterateSteps())
                StepInitialized(boundControl, step);
        }
        if (_editor is { CurrentStep: -1, StepCount: > 0 })
            SetEditorStep(boundControl, 0);
    }

    [MustCallBase(true)]
    protected virtual void StepInitialized(TEditorControl boundControl, TStep step){}

    [MustCallBase(true)]
    protected override void BoundControlExitedTree(TEditorControl boundControl) {}

    public virtual bool SetEditorStep(TEditorControl boundControl, int step)
    {
        return _editor.SetStep(step, boundControl);
    }

    public bool IsEditorStepInRange(int stepIndex)
    {
        return stepIndex >= 0 && stepIndex < _editor.StepCount;
    }
}