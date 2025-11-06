// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.UI.Core;
using Robust.Client.Graphics;

namespace Content.Client._Starlight.CharacterEditor;
public abstract class CharacterEditorMode
{
    [Dependency] private readonly UIEventBus _uiEvents = default!;
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;
    [Dependency] private readonly ILogManager _logMan = default!;
    protected ISawmill Log = default!;

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

    public bool IsActive { get; private set; } = false;

    private CharacterEditor _characterEditor = default!;
    private CharacterEditorModeButton? _modeButton = null;

    private CharacterEditorPanel?[] Panels = new CharacterEditorPanel?[Enum.GetValues<CharacterEditorPanelLayout>().Length];
    protected abstract void RegisterPanels();

    protected TPanel RegisterPanel<TPanel>() where TPanel : CharacterEditorPanel, new()
    {
        var newPanel = _typeFactory.CreateInstance<TPanel>();
        newPanel.Visible = false;
        if (TryGetPanel(newPanel.Layout, out var existing))
        {
            Log.Info($"Panel:{existing} already exists in layout position:{newPanel.Layout}! Overriding!");
            existing.Orphan();
        }
        Panels[(int)newPanel.Layout] = newPanel;

        switch (newPanel.Layout)
        {
            case CharacterEditorPanelLayout.Main:
                _characterEditor.MainPanel.AddChild(newPanel);
                break;
            case CharacterEditorPanelLayout.Side:
                _characterEditor.SidePanel.AddChild(newPanel);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return newPanel;
    }


    private void ChangePanelsVisibility(bool newVisibility)
    {
        foreach (var panel in Panels)
        {
            if (panel != null)
                panel.Visible = newVisibility;
        }
    }

    public IEnumerable<CharacterEditorPanel> IteratePanels()
    {
        foreach (var panel in Panels)
        {
            if (panel != null)
                yield return panel;
        }
    }

    public bool TryGetPanel(CharacterEditorPanelLayout layout, [NotNullWhen(true)] out CharacterEditorPanel? panel)
    {
        panel = Panels[(int)layout];
        return panel != null;
    }
    public void Initialize(CharacterEditor characterEditor)
    {
        _characterEditor = characterEditor;
        Log = _logMan.GetSawmill(GetType().ToString());
        RegisterPanels();
    }

    public void Activate()
    {
        if (IsActive)
            return;
        ChangePanelsVisibility(true);
        IsActive = true;
        if (_modeButton != null)
            _modeButton.Pressed = true;
        _characterEditor.CurrentEditorMode?.Deactivate();
        _characterEditor.CurrentEditorMode = this;
        _uiEvents.RaiseEvent(new CharacterEditorModeEnteredUIEvent(this));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;
        ChangePanelsVisibility(false);
        IsActive = false;
        _uiEvents.RaiseEvent(new CharacterEditorModeExitedUIEvent(this));
        _characterEditor.CurrentEditorMode = null;
    }

    public void LinkButton(CharacterEditorModeButton button)
    {
        if (_modeButton != null)
            throw new InvalidOperationException($"EditorButton already linked to {this}!");
        _modeButton = button;
    }
}

public record struct CharacterEditorModeEnteredUIEvent(CharacterEditorMode NewMode);

public record struct CharacterEditorModeExitedUIEvent(CharacterEditorMode OldMode);