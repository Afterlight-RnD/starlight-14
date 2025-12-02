// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;

public abstract class ProfileEditorStep<TProfile, TProfileEditor, TEditorControl, TEditorSystem, TLayoutEnum> : UISystem
where TProfile: IPersistentProfile, new()
where TEditorControl: ProfileEditorMainControl<TProfileEditor, TLayoutEnum, ProfileEditorStepButton>, new()
where TProfileEditor: ProfileEditor<TProfileEditor, TProfile, TEditorControl, TEditorSystem, TLayoutEnum>, new()
where TEditorSystem: ProfileEditorSystem<TEditorSystem,TEditorControl,TProfile, TProfileEditor, TLayoutEnum>, new()
where TLayoutEnum: struct, Enum
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;

    public int Step { get; private set; } = -1;
    public virtual Type[]? BeforeSteps => null;
    public virtual Type[]? AfterSteps => null;

    private Dictionary<TLayoutEnum, Func<IDynamicTypeFactory,Control>> _panelBuilders = new();
    private event Action<TEditorControl, TProfile>? _handleLoadData;
    private event Action<TEditorControl, TProfile>? _handleSaveData;


    public void INTERNAL_SetStep(int step)
    {
        Step = step;
    }

    public void INTERNAL_SaveData(TProfileEditor editor)
    {
        if (editor.Profile != null)
            _handleLoadData?.Invoke(editor.EditorControl, editor.Profile);
    }

    public void INTERNAL_LoadData(TProfileEditor editor)
    {
        if (editor.Profile != null)
            _handleSaveData?.Invoke(editor.EditorControl, editor.Profile);
    }

    public void INTERNAL_SetupEditor(TProfileEditor editor)
    {
        editor.EditorControl.TryInjectStepControls(Step, _typeFactory, _panelBuilders);
    }

    public void INTERNAL_EditorCreated(TProfileEditor editor)
    {
        EditorCreated(editor);
    }

    protected virtual void EditorCreated(TProfileEditor editor){}

    protected void RegisterPanel<TControl>(TLayoutEnum layout,
        Action<TControl,TProfile> loadData,
        Action<TControl,TProfile> saveData)
        where TControl: Control, new()
    {
        if (_panelBuilders.ContainsKey(layout))
        {
            Log.Warning("Panel was already registered!");
            return;
        }
        _panelBuilders.Add(layout, static typeFact =>
        {
            return typeFact.CreateInstance<TControl>();
        });
        _handleLoadData += (editorControl, profile) =>
        {
            loadData.Invoke(editorControl.GetPanel<TControl>(Step, layout), profile);
        };
        _handleSaveData += (editorControl, profile) =>
        {
            loadData.Invoke(editorControl.GetPanel<TControl>(Step, layout), profile);
        };
    }
}