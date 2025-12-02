// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;
using Content.Client._Starlight.UI;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.Graphics;
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
    }

    [MustCallBase]
    protected override void BoundControlEnteredTree(TEditorControl boundControl)
    {
        EnsureSteps(boundControl);
    }

    [MustCallBase]
    protected override void BoundControlExitedTree(TEditorControl boundControl)
    {
    }

    protected void SubscribeEditorEvent<TEvent>(UIEvent<TEvent> handler)
    where TEvent: struct
    {

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

    private void EnsureSteps(TEditorControl boundControl)
    {
        if (boundControl.StepControlsInjected) return;
        foreach (var step in _steps)
            step.InjectControls(boundControl);

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
            var newButton = new TStepButton();
            newButton.SetFromStep(i, step.StepName, step.StepDescription, step.StepIcon);
            boundControl.AddStepButton(newButton);
        }
        boundControl.StepControlsInjected = true;
    }


}

public interface IProfileEditorStep<TProfile, TProfileEditor, in TEditorControl>
    where TProfile : class, IPersistentProfile, new()
    where TProfileEditor: ProfileEditor<TProfileEditor,TProfile, TEditorControl>, new()
    where TEditorControl : SLControl, IProfileEditorMainControl<TEditorControl,TProfileEditor>, new()
{
    public IReadOnlySet<Type>? BeforeSteps { get; }
    public IReadOnlySet<Type>? AfterSteps { get; }

    public string StepName { get; }
    public string? StepDescription => null;
    public Texture? StepIcon => null;

    public void InjectControls(TEditorControl boundControl);
};

public abstract class ProfileEditorStep<TProfile, TProfileEditor,TEditorControl, TPanelEnum> : UISystem,
    IProfileEditorStep<TProfile, TProfileEditor,TEditorControl>
    where TProfile : class, IPersistentProfile, new()
    where TProfileEditor: ProfileEditor<TProfileEditor,TProfile, TEditorControl>, new()
    where TEditorControl : ProfileEditorMainControl<TEditorControl, TProfileEditor, TPanelEnum>, new()
    where TPanelEnum : struct, Enum, IConvertible
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;

    public IReadOnlySet<Type>? BeforeSteps => _beforeSteps;
    public IReadOnlySet<Type>? AfterSteps => _afterSteps;

    private HashSet<Type>? _beforeSteps = null;
    private HashSet<Type>? _afterSteps = null;
    public abstract string StepName { get; }
    private Dictionary<TPanelEnum, Type> _panelRegistrations = new();

    public override void Initialize()
    {
        base.Initialize();
        Setup();
    }

    public abstract void Setup();

    void IProfileEditorStep<TProfile, TProfileEditor, TEditorControl>.InjectControls(TEditorControl boundControl)
    {
        foreach (var (panelEnum, type) in _panelRegistrations)
        {
            var panel = _typeFactory.CreateInstance<SLControl>(type);
            boundControl.InjectPanel(panelEnum, panel);
        }
        boundControl.StepControlsInjected = true;
    }

    protected void RegisterAfterStep<TOtherStep>()
        where TOtherStep : ProfileEditorStep<TProfile, TProfileEditor,TEditorControl, TPanelEnum>, new()
    {
        _afterSteps ??= new();
        _afterSteps.Add(typeof(TOtherStep));
    }

    protected void RegisterBeforeStep<TOtherStep>()
        where TOtherStep : ProfileEditorStep<TProfile, TProfileEditor, TEditorControl, TPanelEnum>, new()
    {
        _beforeSteps ??= new();
        _beforeSteps.Add(typeof(TOtherStep));
    }

    protected void RegisterPanel<TPanel>(TPanelEnum panelEnum)
        where TPanel : SLControl, new()
    {
        if (!_panelRegistrations.TryAdd(panelEnum, typeof(TPanel)))
            Log.Error($"Duplicate panel registration:{typeof(TPanel)} in position:{panelEnum}");
    }
}