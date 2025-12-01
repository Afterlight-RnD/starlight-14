// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT


using Content.Client._Starlight.ProfileEditor.UI;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Serialization.Manager.Exceptions;


namespace Content.Client._Starlight.ProfileEditor;

public interface IProfileEditor
{
    public bool HasEdits { get; }
    public void Initialize(List<Control> panels,
        IEntitySystemManager systemManager,
        IResourceCache resCache,
        IDynamicTypeFactory typeFactory,
        ISawmill log);
    public Type GetPanelBaseType { get; }

    public void MarkChanged();

    public void ClearEdits(bool discard = true);

    public bool IsOpen { get; }

    public void Open();

    public void Close(bool discard = true);
};

public abstract class ProfileEditor<TSelf,TProfile, TEditorControl, TSystem,TBasePanel, TStepButton, TStepEnum, TPosEnum> : IProfileEditor
where TSystem: EntitySystem, IProfileEditorSystem, new()
where TSelf: ProfileEditor<TSelf,TProfile, TEditorControl, TSystem, TBasePanel, TStepButton, TStepEnum, TPosEnum>, IProfileEditor, new()
where TProfile: class, IPersistentProfile, new()
where TEditorControl: ProfileEditorMainControl<TSelf>, new()
where TBasePanel: ProfileEditorPanelBaseControl<TSelf, TEditorControl, TStepEnum, TPosEnum>
where TStepButton: ProfileEditorStepButton<TStepEnum>, new()
where TStepEnum: struct, Enum, IConvertible
where TPosEnum: struct, Enum, IConvertible
{
    public bool HasEdits { get; private set; }
    public TSystem LinkedSystem { get; private set; } = default!;
    public TEditorControl EditorControl { get; }= new TEditorControl();
    public TStepEnum CurrentStep { get; private set; }
    public virtual TStepEnum FirstStep => default;
    public Type GetPanelBaseType => typeof(TBasePanel);
    public int StepCount { get;}
    private List<List<TBasePanel?>> _steps = new();

    public bool IsOpen { get; private set; }

    public ProfileEditor()
    {
        StepCount = Enum.GetValues<TStepEnum>().Length;
        var temp = new TBasePanel?[ Enum.GetValues<TPosEnum>().Length];
        for (var i = 0; i < StepCount; i++)
        {
            _steps.Add(new List<TBasePanel?>(temp));
        }
    }

    public void MarkChanged()
    {
        HasEdits |= true;
    }

    public void ClearEdits(bool discard = true)
    {
        if (discard)
            Reset();
        else
            Apply();
        HasEdits = false;
    }

    public void Open()
    {
        if (IsOpen)
            return;
        Entered();
        EditorControl.Visible = true;
        IsOpen = true;
    }

    public void Close(bool discard = true)
    {
        if (!IsOpen)
            return;
        EditorControl.Visible = false;
        Exited();
        IsOpen = false;
        if (HasEdits)
            ClearEdits(discard);
    }


    public void Initialize(List<Control> panels, IEntitySystemManager systemManager,
        IResourceCache resCache, IDynamicTypeFactory typeFactory, ISawmill log)
    {
        if (!systemManager.TryGetEntitySystem<TSystem>(out var found))
        {
            log.Fatal($"Could not find entity system for profile editor! Or tried to initialize outside of simulation!");
            throw new Exception();
        }
        LinkedSystem = found;
        LinkedSystem.RegisterEditor(this);

        foreach (var control in panels)
        {
            if (control is not TBasePanel panel)
                throw new GenericParameterMismatchException();
            if (!TryRegisterPanel(panel))
            {
                log.Error($"tried to register Panel:{panel} but there is already a " +
                          $"panel in pos:{panel.PanelPosition}, for step:{panel.Step}!");
                continue;
            }
            panel.Visible = false;
            panel.Initialize((TSelf)this,EditorControl);
        }

        foreach (var step in Enum.GetValues<TStepEnum>())
        {
            var button = typeFactory.CreateInstance<TStepButton>();
            button.ToggleMode = true;
            button.Group = EditorControl.StepSelectorGroup;
            button.SetFromStep(step, resCache);
            button.OnToggled += OnStepSelectorToggled;
        }
        foreach (var step in GetPanelsForStep(CurrentStep))
        {
            if (step == null) continue;
            step.Visible = true;
            step.Activate();
        }
        if (!EqualityComparer<TStepEnum>.Default.Equals(FirstStep, CurrentStep))
            SetStep(FirstStep);
    }

    private void OnStepSelectorToggled(BaseButton.ButtonToggledEventArgs obj)
    {
        if (obj.Pressed)
            SetStep(((TStepButton)obj.Button).Step);
    }

    private bool TryRegisterPanel(TBasePanel panel)
    {
        var step = panel.Step.ToInt32(null);
        var pos = panel.PanelPosition.ToInt32(null);
        if (_steps[step][pos] != null)
            return false;
        _steps[step][pos] = panel;
        return true;
    }

    public void SetStep(TStepEnum step)
    {
        if (EqualityComparer<TStepEnum>.Default.Equals(step, CurrentStep))
            return;
        var currentPanels = GetPanelsForStep(CurrentStep);
        var nextPanels = GetPanelsForStep(step);
        for (var i = 0; i < nextPanels.Count; i++)
        {
            var currentPanel = currentPanels[i];
            var nextPanel = nextPanels[i];
            if (currentPanel == nextPanel) continue;
            if (currentPanel != null)
            {
                currentPanel.Visible = false;
                currentPanel.Deactivate();
            }
            if (nextPanel != null)
            {
                nextPanel.Visible = true;
                nextPanel.Activate();
            }
        }

        var previousStep = step;
        CurrentStep = step;
        StepChanged(previousStep);
    }

    protected List<TBasePanel?> GetPanelsForStep(TStepEnum step)
    {
        return _steps[step.ToInt32(null)];
    }

    protected virtual void Entered(){}
    protected virtual void Exited(){}

    protected virtual void Reset(){}

    protected virtual void Apply(){}

    public virtual void StepChanged(TStepEnum previousStep){}
}