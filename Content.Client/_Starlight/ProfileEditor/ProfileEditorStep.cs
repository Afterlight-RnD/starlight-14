// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;

public interface IProfileEditorStep
{
    public Type[]? BeforeSteps { get; }
    public Type[]? AfterSteps { get; }
}


public abstract class ProfileEditorStep<TEditorControl> : IProfileEditorStep
    where TEditorControl: Control, ISLControl, IProfileEditorControl
{
    public virtual Type[]? BeforeSteps => null;
    public virtual Type[]? AfterSteps => null;

    public abstract void InjectStepControls(TEditorControl editorControl);

    public abstract void RemoveStepControls(TEditorControl editorControl);

    public virtual void StepEntered(TEditorControl editorControl){}

    public virtual void StepExited(TEditorControl editorControl){}
}