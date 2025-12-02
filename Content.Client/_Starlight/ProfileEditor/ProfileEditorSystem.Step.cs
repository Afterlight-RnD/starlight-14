// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;

public abstract class ProfileEditorStep<TProfile, TProfileEditor, TEditorControl, TEditorSystem, TLayoutEnum, TStepButton> : UISystem
where TProfile: class,IPersistentProfile<TProfile>
where TEditorControl: ProfileEditorMainControl<TProfileEditor, TProfile,TLayoutEnum, TStepButton>, new()
where TProfileEditor: ProfileEditor<TProfileEditor, TProfile, TEditorControl, TEditorSystem, TLayoutEnum, TStepButton>, new()
where TEditorSystem: ProfileEditorSystem<TEditorSystem,TEditorControl,TProfile, TProfileEditor, TLayoutEnum, TStepButton>, new()
where TStepButton:ProfileEditorStepButton, new()
where TLayoutEnum: struct, Enum
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;

    public abstract string StepName { get; }
    public virtual string? StepDescription => null;
    public virtual Texture? StepIcon => null;

    public int Step { get; private set; } = -1;
    public virtual Type[]? BeforeSteps => null;
    public virtual Type[]? AfterSteps => null;

    private Dictionary<TLayoutEnum, Func<IDynamicTypeFactory,Control>> _panelBuilders = new();
    private event Action<TEditorControl, TProfile>? _handleLoadData;
    private event Action<TEditorControl, TProfile>? _handleSaveData;

    private ProfileEditorPanelBuilder<TProfileEditor, TProfile, TLayoutEnum, TEditorControl, TStepButton> _builder =
        default!;

    public override void Initialize()
    {
        _builder = new ProfileEditorPanelBuilder<TProfileEditor, TProfile, TLayoutEnum, TEditorControl, TStepButton>(
            Log,
            _typeFactory);
        base.Initialize();
    }

    public void INTERNAL_SetupStep(int step)
    {
        Step = step;
        SetupStep(_builder);
    }

    public void INTERNAL_SaveData(TProfileEditor editor)
    {
        _handleLoadData?.Invoke(editor.EditorControl, editor.Profile);
    }

    public void INTERNAL_LoadData(TProfileEditor editor)
    {
        _handleSaveData?.Invoke(editor.EditorControl, editor.Profile);
    }

    public void INTERNAL_SetupEditor(TProfileEditor editor)
    {
        editor.EditorControl.TryInjectStepControls(Step,StepName, StepDescription, StepIcon, _builder);
    }

    public void INTERNAL_EditorCreated(TProfileEditor editor)
    {
        EditorCreated(editor);
    }

    protected abstract void SetupStep(IProfileEditorPanelBuilder<TProfile,TLayoutEnum> panelBuilder);

    protected virtual void EditorCreated(TProfileEditor editor){}
}