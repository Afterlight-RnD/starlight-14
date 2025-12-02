// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.ProfileEditor.UI;
using Content.Client._Starlight.UI;
using Content.Shared._Starlight.Abstract.Extensions;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;

public interface IProfileEditor
{
    public bool IsOpen { get;}
    public void FinishSetup(Control editorRoot);
    public int StepCount { get; }

    public int LayoutCount { get; }

    public int CurrentStep { get; }

    public void Open();
    public void Close();

    public void SetStep(int step);
};

public interface IProfileEditor<TSelf,out TEditorControl, TLayoutEnum> : IProfileEditor
    where TSelf: IProfileEditor<TSelf, TEditorControl, TLayoutEnum>, new()
    where TEditorControl: SLControl, IProfileEditorMainControl<TSelf>, new()
    where TLayoutEnum: struct, Enum
{
    public TEditorControl EditorControl { get; }

}
public abstract class ProfileEditor<TSelf,TProfile, TEditorControl, TSystem, TLayoutEnum, TStepButton> : IProfileEditor<TSelf,TEditorControl, TLayoutEnum>
where TSelf:ProfileEditor<TSelf,TProfile, TEditorControl, TSystem, TLayoutEnum, TStepButton>, new()
where TProfile: IPersistentProfile, new()
where TEditorControl: ProfileEditorMainControl<TSelf, TLayoutEnum, TStepButton>, new()
where TSystem: ProfileEditorSystem<TSystem, TEditorControl,TProfile, TSelf, TLayoutEnum, TStepButton>, new()
where TStepButton: ProfileEditorStepButton, new()
where TLayoutEnum: struct, Enum
{
    [Dependency] protected readonly IEntityManager EntityManager = default!;
    public int StepCount => EditorSystem.StepCount;
    public int CurrentStep { get; private set; } = 0;
    public int LayoutCount { get; } = Enum.GetValues<TLayoutEnum>().Length;
    public TEditorControl EditorControl { get; }
    public TProfile Profile { get; private set; }
    public bool IsOpen { get; private set; } = false;
    protected Control? _editorRootControl = null;
    public TSystem EditorSystem { get; private set; }

    private IProfileEditorSystem<TProfile, TSelf> GetEditorInterface => EditorSystem;
    protected ProfileEditor()
    {
        IoCManager.InjectDependencies(this);
        if (!EntityManager.TryGetDependencyCollection(out var systemDeps))
            throw new InvalidOperationException($"Tried to create profile editor:{GetType()} out of sim!");
        EditorSystem = systemDeps.Resolve<TSystem>();
        var self = (TSelf)this;
        var editorInterface = GetEditorInterface;
        var layoutSize = Enum.GetValues<TLayoutEnum>().Length;
        EditorControl = new(){Editor = self, StepControls = new Control[StepCount, layoutSize]};
        Profile = new();
        editorInterface.EditorCreated(self);
    }
    public void FinishSetup(Control editorRoot)
    {
        _editorRootControl = editorRoot;
        SetStep(CurrentStep);
    }

    public virtual void Initialize(){}

    public void Open()
    {
        if (IsOpen)
            return;
        if (_editorRootControl == null)
            throw new InvalidOperationException("EditorRootControl must be defined!");
        _editorRootControl.AddChild(EditorControl);
        IsOpen = true;
    }

    public void Close()
    {
        if (!IsOpen || _editorRootControl == null)
            return;
        EditorControl.Orphan();
        IsOpen = false;
    }

    public void SetStep(int step)
    {
        if (CurrentStep == step)
            return;
        EditorControl.ToggleStepControls(CurrentStep, false);
        EditorControl.ToggleStepControls(CurrentStep, true);
        CurrentStep = step;
    }
}