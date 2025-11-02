// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Controls;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.CharacterEditor;

public interface ICharacterEditorMode
{
    public void AddPanelsToUI(params CharacterEditorPanelStub[] parentControls);

    public void Initialize();

    public void RemovePanelsFromUI();

    public void Activate();

    public void Deactivate();
}


public abstract class CharacterEditorMode : ICharacterEditorMode
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;
    private Control?[] editorPanels;
    public bool IsActive { get; private set; } = false;

    /// <summary>
    /// Which modes should be put before this one in the character editor
    /// </summary>
    public virtual Type[]? BeforeModes => null;
    /// <summary>
    /// Which modes should be put after this one in the character editor
    /// </summary>
    public virtual Type[]? AfterModes => null;

    public virtual Texture? ModeIcon => null;

    public abstract string ModeName { get; }

    public virtual string? ModeSubText => null;

    public abstract void SetupPanels();


    protected CharacterEditorMode()
    {
        editorPanels = new Control?[Enum.GetValues<CharacterEditorPanelLayout>().Length];
    }

    void ICharacterEditorMode.Initialize()
    {
        SetupPanels();
    }

    public Control? GetPanel(CharacterEditorPanelLayout layout)
    {
        return editorPanels[(int)layout];
    }

    public T? GetPanel<T>(CharacterEditorPanelLayout layout) where T: Control
    {
        var control = editorPanels[(int)layout];
        if (control is T retVal)
            return retVal;
        return null;
    }

    public IEnumerable<Control> IteratePanels()
    {
        foreach (var panel in editorPanels)
        {
            if (panel == null)
                continue;
            yield return panel;
        }
    }

    /// <summary>
    /// Injects editor panel controls into specified parents.
    /// </summary>
    /// <param name="parentControls"></param>
    /// <exception cref="Exception"></exception>
    void ICharacterEditorMode.AddPanelsToUI(params CharacterEditorPanelStub[] parentControls)
    {
        if (parentControls.Length != editorPanels.Length)
            throw new Exception($"EditorPanel to ParentControl Mismatch! PanelCount:{editorPanels.Length} Parents:{parentControls.Length}");

        foreach (var control in parentControls)
        {
            var panel = GetPanel(control.EditorLayout);
            if (panel == null)
                continue;
            if (panel.Parent != null)
                throw new Exception(
                    $"Panel of type:{panel.GetType()} in editorMode:{GetType()} is already already injected!");
            control.AddChild(panel);
        }

    }

    void ICharacterEditorMode.RemovePanelsFromUI()
    {
        foreach (var panel in editorPanels)
            panel?.Orphan();
    }

    void ICharacterEditorMode.Activate()
    {
        if (IsActive)
            return;
        foreach (var panel in editorPanels)
        {
            if (panel != null)
                panel.Visible = true;
        }
        IsActive = true;
    }

    void ICharacterEditorMode.Deactivate()
    {
        if (!IsActive)
            return;
        foreach (var panel in editorPanels)
        {
            if (panel != null)
                panel.Visible = false;
        }
        IsActive = false;
    }

    protected void RegisterPanel<T>(CharacterEditorPanelLayout panelLayout) where T : Control, new()
    {
        var panel = _typeFactory.CreateInstance<T>();
        var panelIdx = (int)panelLayout;
        if (editorPanels[panelIdx] != null)
            throw new InvalidOperationException(
                $"Could not set Panel of type{typeof(T)} location:{panelLayout} is already occupied!");
        editorPanels[panelIdx] = panel;
    }
}

public record struct CharacterEditorModeEnteredUIEvent(CharacterEditorMode NewMode);

public record struct CharacterEditorModeExitedUIEvent(CharacterEditorMode OldMode);