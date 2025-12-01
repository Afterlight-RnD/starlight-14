// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;

namespace Content.Client._Starlight.ProfileEditor.UI;

public abstract class ProfileEditorPanelBaseControl<TEditor,TMainEditorControl, TStepEnum, TPosEnum> : SLBox
where TEditor : class, IProfileEditor, new()
where TMainEditorControl: ProfileEditorMainControl<TEditor>, new()
where TStepEnum: struct, Enum
where TPosEnum: struct, Enum
{
    public TEditor Editor { get; private set; } = default!;
    public TMainEditorControl MainControl { get; private set; } = default!;

    public abstract TStepEnum Step { get; }
    public abstract TPosEnum PanelPosition{ get; }

    protected abstract void InjectPanel(TMainEditorControl editorControl);

    public virtual void Activate(){}

    public virtual void Deactivate(){}

    public void Initialize(TEditor editor, TMainEditorControl editorControl)
    {
        Editor = editor;
        MainControl = editorControl;
        InjectPanel(editorControl);
    }
}