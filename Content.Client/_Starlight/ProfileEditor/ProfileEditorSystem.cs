// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;
using Content.Client._Starlight.UI;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Shared.Reflection;

namespace Content.Client._Starlight.ProfileEditor;

public abstract class ProfileEditorSystem<TProfile, TProfileEditor, TEditorControl, TStepButton> : BoundUISystem<TEditorControl>
    where TProfileEditor: ProfileEditor<TProfileEditor,TProfile, TEditorControl>, new()
    where TProfile : class, IPersistentProfile, new()
    where TEditorControl : SLControl, IProfileEditorMainControl<TEditorControl,TProfileEditor>, new()
    where TStepButton : ProfileEditorStepButton, new()
{
    [Dependency] private IReflectionManager _reflectionManager = default!;

    public int StepCount => _steps.Count;
    private List<IProfileEditorStep<TProfile, TProfileEditor, TEditorControl>> _steps = new();

    public override void Initialize()
    {
        base.Initialize();
        foreach (var profileStep in _reflectionManager.GetAllChildren<IProfileEditorStep<TProfile, TProfileEditor ,TEditorControl>>())
            _steps.Add(
                (IProfileEditorStep<TProfile, TProfileEditor,TEditorControl>)EntityManager.EntitySysManager.GetEntitySystem(
                    profileStep));
        InitSteps();
    }

    [MustCallBase]
    protected override void BoundControlEnteredTree(TEditorControl boundControl)
    {
        var editorInterface = (IProfileEditor<TProfileEditor>)boundControl.Editor;
        editorInterface.InitFromSystem(StepCount);
        InjectStepControls(boundControl);
        foreach (var step in _steps)
        {
            step.Setup(boundControl.Editor);
            boundControl.Editor.OnDataLoaded += step.LoadData;
            boundControl.Editor.OnDataSaved += step.SaveData;
            if (editorInterface.CurrentStep == -1)
                editorInterface.SetCurrentStep(0);
        }
        if ( boundControl.Editor.EditingProfile != null)
            _steps[boundControl.Editor.CurrentStep].LoadData(boundControl, boundControl.Editor.EditingProfile);
    }

    [MustCallBase]
    protected override void BoundControlExitedTree(TEditorControl boundControl)
    {
        foreach (var step in _steps)
        {
            step.ShutDown(boundControl.Editor);
            boundControl.Editor.OnDataLoaded -= step.LoadData;
            boundControl.Editor.OnDataSaved -= step.SaveData;
        }
    }

    public void RaiseEditorEvent<TEvent>(TProfileEditor editor, TEvent args)
        where TEvent : struct
    {
        editor.RaiseEditorEvent(args);
    }

    public void RaiseEditorControlEvent<TEvent>(TProfileEditor editor, TEvent args)
        where TEvent : struct
    {
        editor.RaiseControlEditorEvent(args);
    }

    private void InitSteps()
    {
        _steps.Sort(((step1, step2) =>
        {
            var step1Type = step1.GetType();
            var step2Type = step2.GetType();
            if (step1.BeforeSteps != null && step1.BeforeSteps.Contains(step2Type))
                return -1;
            if (step2.AfterSteps != null && step2.AfterSteps.Contains(step1Type))
                return -1;
            if (step1.AfterSteps != null && step1.AfterSteps.Contains(step2Type))
                return 1;
            if (step2.BeforeSteps != null && step2.BeforeSteps.Contains(step1Type))
                return 1;
            return 0;
        }));
        for (var i = 0; i < _steps.Count; i++)
        {
            var step = _steps[i];
            step.InitStep(i);
        }
    }

    private void InjectStepControls(TEditorControl boundControl)
    {
        if (boundControl.StepControlsInjected)
            return;
        for (var i = 0; i < _steps.Count; i++)
        {
            var newButton = new TStepButton();
            var step = _steps[i];
            newButton.SetFromStep(i, step.StepName, step.StepDescription, step.StepIcon);
            boundControl.AddStepButton(newButton);
            step.InjectControls(boundControl);
        }
        boundControl.StepControlsInjected = true;
    }


}

public interface IProfileEditorStep<TProfile, TProfileEditor, in TEditorControl>
    where TProfile : class, IPersistentProfile, new()
    where TProfileEditor: ProfileEditor<TProfileEditor,TProfile, TEditorControl>, new()
    where TEditorControl : SLControl, IProfileEditorMainControl<TEditorControl,TProfileEditor>, new()
{
    public int Step { get; }
    public IReadOnlySet<Type>? BeforeSteps { get; }
    public IReadOnlySet<Type>? AfterSteps { get; }

    public string StepName { get; }
    public string? StepDescription => null;
    public Texture? StepIcon => null;

    public void InitStep(int step);

    public void InjectControls(TEditorControl boundControl);

    public void LoadData(TEditorControl editorControl, TProfile profile);

    public void SaveData(TEditorControl editorControl, TProfile profile);

    public void Setup(TProfileEditor editor);

    public void ShutDown(TProfileEditor editor);
};

public abstract class ProfileEditorStep<TProfile, TProfileEditor,TEditorControl, TPanelEnum> : UISystem,
    IProfileEditorStep<TProfile, TProfileEditor,TEditorControl>
    where TProfile : class, IPersistentProfile, new()
    where TProfileEditor: ProfileEditor<TProfileEditor,TProfile, TEditorControl>, new()
    where TEditorControl : ProfileEditorMainControl<TEditorControl, TProfileEditor, TPanelEnum>, new()
    where TPanelEnum : struct, Enum, IConvertible
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;

    public int Step { get; set; }
    public IReadOnlySet<Type>? BeforeSteps => _beforeSteps;
    public IReadOnlySet<Type>? AfterSteps => _afterSteps;

    private HashSet<Type>? _beforeSteps = null;
    private HashSet<Type>? _afterSteps = null;
    public abstract string StepName { get; }
    private Dictionary<TPanelEnum, Type> _panelRegistrations = new();
    private event Action<TEditorControl, TProfile>? HandleSave;
    private event Action<TEditorControl, TProfile>? HandleLoad;

    void IProfileEditorStep<TProfile, TProfileEditor,TEditorControl>.LoadData(TEditorControl editorControl, TProfile profile)
    {
        HandleLoad?.Invoke(editorControl, profile);
    }

     void IProfileEditorStep<TProfile, TProfileEditor,TEditorControl>.SaveData(TEditorControl editorControl, TProfile profile)
    {
        HandleSave?.Invoke(editorControl, profile);
    }

    void IProfileEditorStep<TProfile, TProfileEditor,TEditorControl>.Setup(TProfileEditor editor)
    {
        SetupStep(editor);
        editor.OnStepEntered += HandleStepEnter;
        editor.OnStepExited += HandleStepExit;
    }

    void IProfileEditorStep<TProfile, TProfileEditor,TEditorControl>.ShutDown(TProfileEditor editor)
    {
        editor.OnStepEntered -= HandleStepEnter;
        editor.OnStepExited -= HandleStepExit;
        ShutDownStep(editor);
    }

    public virtual void ShutDownStep(TProfileEditor editor){}

    private void HandleStepExit(TProfileEditor editor, int step)
    {
        if (step != Step)
            return;
        StepExited(editor);
    }

    private void HandleStepEnter(TProfileEditor editor, int step)
    {
        if (step != Step)
            return;
        StepEntered(editor);
    }

    protected abstract void SetupStep(TProfileEditor editor);

    public virtual void StepEntered(TProfileEditor editor){}

    public virtual void StepExited(TProfileEditor editor){}

    void IProfileEditorStep<TProfile, TProfileEditor,TEditorControl>.InitStep(int step)
    {
        Step = step;
    }

    void IProfileEditorStep<TProfile, TProfileEditor, TEditorControl>.InjectControls(TEditorControl boundControl)
    {
        foreach (var (panelEnum, type) in _panelRegistrations)
        {
            var panel = _typeFactory.CreateInstance<SLControl>(type);
            boundControl.RegisterPanel(Step,panelEnum, panel);
        }
        boundControl.StepControlsInjected = true;
    }

    protected void AfterStep<TOtherStep>()
        where TOtherStep : ProfileEditorStep<TProfile, TProfileEditor,TEditorControl, TPanelEnum>, new()
    {
        _afterSteps ??= new();
        _afterSteps.Add(typeof(TOtherStep));
    }

    protected void BeforeStep<TOtherStep>()
        where TOtherStep : ProfileEditorStep<TProfile, TProfileEditor, TEditorControl, TPanelEnum>, new()
    {
        _beforeSteps ??= new();
        _beforeSteps.Add(typeof(TOtherStep));
    }

    protected void RegisterPanel<TPanel>(TPanelEnum panelEnum,
        Action<TPanel, TProfile> loadProfile,
        Action<TPanel, TProfile> saveProfile)
        where TPanel : Control, new()
    {
        if (!_panelRegistrations.TryAdd(panelEnum, typeof(TPanel)))
            Log.Error($"Duplicate panel registration:{typeof(TPanel)} in position:{panelEnum}");
        HandleSave += (editorControl, profile) =>
        {
            saveProfile.Invoke(editorControl.GetPanel<TPanel>(panelEnum), profile);
        };
        HandleLoad += (editorControl, profile) =>
        {
            loadProfile.Invoke(editorControl.GetPanel<TPanel>(panelEnum), profile);
        };
    }
}