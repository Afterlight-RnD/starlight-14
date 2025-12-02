// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;

public abstract class ProfileEditorRootControl<TProfileEditor, TEditorControl> : Control
where TProfileEditor: class, IProfileEditor<TProfileEditor, TEditorControl>, new()
where TEditorControl: SLControl,IProfileEditorMainControl<TEditorControl, TProfileEditor>, new()
{
    public bool IsOpen => EditorControl.IsInsideTree;
    private TProfileEditor? _profileEditor = null;
    public TEditorControl EditorControl => ProfileEditor.EditorControl;
    public TProfileEditor ProfileEditor
    {
        get
        {
            if (_profileEditor != null)
                return _profileEditor;
            _profileEditor = new();
            _profileEditor.Initialize(new TEditorControl());
            return _profileEditor;
        }
    }

    public void Open()
    {
        if (IsOpen)
            return;
        AddChild(EditorControl);
    }

    public void Close()
    {
        if (!IsOpen)
            return;
        EditorControl.Orphan();
    }
}