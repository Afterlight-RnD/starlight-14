// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface;
using Robust.Shared.Reflection;

namespace Content.Client._Starlight.ProfileEditor;

public sealed class ProfileEditorSystem : UISystem
{
    [Dependency] private readonly IReflectionManager _reflectionManager = default!;
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;

    private Dictionary<Type, IProfileEditor> _editors = new();
    private Dictionary<Type, IProfileEditor> _editorControlLinks = new();

    public TEditor GetProfileEditor<TEditor>()
        where TEditor: IProfileEditor, new()
    {
        return (TEditor)_editors[typeof(TEditor)];
    }

    public void SetupEditorForControl<TEditorControl>(TEditorControl editorControl)
    where TEditorControl: Control, IProfileEditorControl
    {
        if (!_editorControlLinks.TryGetValue(typeof(TEditorControl), out var editor) || editor.IsSetup) return;
        foreach (var stepType in _reflectionManager.GetAllChildren<IProfileEditorStep>())
        {
            var step = _typeFactory.CreateInstance<IProfileEditorStep>(stepType, true);
            editor.RegisterStep(step);
        }
        editor.FinishSetup();
    }

    public void CleanupEditorForControl<TEditorControl>(TEditorControl editorControl)
        where TEditorControl : Control, IProfileEditorControl
    {
        if (!_editorControlLinks.Remove(typeof(TEditorControl), out var editor) || !editor.IsSetup) return;
        editor.CleanupSteps();
    }

    public override void Initialize()
    {
        DiscoverEditors();
    }

    private void DiscoverEditors()
    {
        foreach (var editorType in _reflectionManager.GetAllChildren<IProfileEditor>())
        {
            var editor = _typeFactory.CreateInstance<IProfileEditor>(editorType, true);
            _editors.Add(editorType, editor);
            if (!_editorControlLinks.TryAdd(editor.EditorControlType, editor))
            {
                Log.Error($"ProfileEditor of type:{editorType} cannot be linked to controlType:{editor.EditorControlType} because ControlType is already linked to editor of type:{_editorControlLinks[editor.EditorControlType].GetType()}");
            }
        }
    }
}

public abstract class ProfileEditorSystem<TEditorControl, TEditor> : BoundUISystem<TEditorControl>
    where TEditorControl: Control, IProfileEditorControl, new()
    where TEditor : class, IProfileEditor, new()
{
    [Dependency] private readonly ProfileEditorSystem _profileEditor = default!;

    private TEditor? _cachedEditor = null;

    public TEditor Editor => _cachedEditor ??= _profileEditor.GetProfileEditor<TEditor>();

    public int CurrentStepIndex => Editor.CurrentStepIndex;

    public bool SetStep(int stepIndex)
    {
        var oldStep = Editor.CurrentStepIndex;
        if (!Editor.SetStep(stepIndex))
            return false;
        OnStepChanged(oldStep, stepIndex);
        return true;
    }

    protected virtual void OnStepChanged(int oldIndex, int newIndex)
    {
    }

    protected override void BoundControlEnteredTree(TEditorControl boundControl)
    {
        _profileEditor.SetupEditorForControl(boundControl);

    }

    protected override void BoundControlExitedTree(TEditorControl boundControl)
    {
        _profileEditor.CleanupEditorForControl(boundControl);
    }
}