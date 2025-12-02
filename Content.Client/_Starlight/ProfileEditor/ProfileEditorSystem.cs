// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Client._Starlight.ProfileEditor.UI;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Shared.Reflection;

namespace Content.Client._Starlight.ProfileEditor;

public interface IProfileEditorSystem<TProfile, in TProfileEditor>
    where TProfile : class, IPersistentProfile
    where TProfileEditor : IProfileEditor, new()
{
    public void EditorCreated(TProfileEditor editor);

    public void SaveData(TProfileEditor editor);

    public void LoadData(TProfileEditor editor);
}

public abstract class ProfileEditorSystem<TSelf, TEditorControl, TProfile, TProfileEditor, TLayoutEnum, TStepButton> : UISystem,
    IProfileEditorSystem<TProfile, TProfileEditor>
    where TSelf : ProfileEditorSystem<TSelf, TEditorControl, TProfile, TProfileEditor, TLayoutEnum, TStepButton>, new()
    where TEditorControl : ProfileEditorMainControl<TProfileEditor, TProfile,TLayoutEnum, TStepButton>, new()
    where TProfile : class, IPersistentProfile<TProfile>
    where TProfileEditor : ProfileEditor<TProfileEditor, TProfile, TEditorControl, TSelf, TLayoutEnum, TStepButton>, new()
    where TStepButton : ProfileEditorStepButton, new()
    where TLayoutEnum : struct, Enum
{
    [Dependency] private readonly IReflectionManager _reflectionManager = default!;
    private List<ProfileEditorStep<TProfile, TProfileEditor, TEditorControl, TSelf, TLayoutEnum, TStepButton>> _stepSystems = new();

    public abstract TProfile CreateEditorProfile();

    public int StepCount => _stepSystems.Count;

    protected Type GetStepBaseType => typeof(ProfileEditorStep<TProfile, TProfileEditor, TEditorControl, TSelf, TLayoutEnum, TStepButton>);

    public override void Initialize()
    {
        InitStepSystems();
        base.Initialize();
    }

    void IProfileEditorSystem<TProfile, TProfileEditor>.EditorCreated(TProfileEditor editor)
    {
        foreach (var step in _stepSystems)
            step.INTERNAL_SetupEditor(editor);
        editor.Initialize();
        EditorCreated(editor);
        foreach (var step in _stepSystems)
        {
            step.INTERNAL_EditorCreated(editor);
        }
    }

    protected virtual void EditorCreated(TProfileEditor editor)
    {
    }

    public void SaveData(TProfileEditor editor)
    {
        foreach (var step in _stepSystems)
        {
            step.INTERNAL_SaveData(editor);
        }
    }

    public void LoadData(TProfileEditor editor)
    {
        foreach (var step in _stepSystems)
        {
            step.INTERNAL_LoadData(editor);
        }
    }

    private void InitStepSystems()
    {
        foreach (var stepType in _reflectionManager.GetAllChildren(GetStepBaseType))
        {
            _stepSystems.Add(
                (ProfileEditorStep<TProfile, TProfileEditor, TEditorControl, TSelf, TLayoutEnum, TStepButton>)
                EntityManager.EntitySysManager.GetEntitySystem(stepType));
        }
        _stepSystems.Sort(((step1, step2) =>
        {
            var step1Type = step1.GetType();
            var step2Type = step2.GetType();
            if (step1.BeforeSteps != null && step1.BeforeSteps.Contains(step2Type))
            {
                return -1;
            }
            if (step2.BeforeSteps != null && step2.BeforeSteps.Contains(step1Type))
            {
                return 1;
            }
            if (step1.AfterSteps != null && step1.AfterSteps.Contains(step2Type))
            {
                return 1;
            }
            if (step2.AfterSteps != null && step2.AfterSteps.Contains(step1Type))
            {
                return -1;
            }
            return 0;
        }));
        for (var i = 0; i < StepCount; i++)
            _stepSystems[i].INTERNAL_SetupStep(i);
    }
}