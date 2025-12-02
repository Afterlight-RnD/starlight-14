// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;

public abstract class ProfileEditorRootControl<TProfileEditor, TProfile,TEditorControl, TLayoutEnum, TStepButton> : Control
where TProfileEditor: IProfileEditor<TProfileEditor,TProfile,TEditorControl,TLayoutEnum, TStepButton>, new()
where TEditorControl: ProfileEditorMainControl<TProfileEditor, TProfile,TLayoutEnum, TStepButton>, new()
where TStepButton: ProfileEditorStepButton, new()
where TProfile: class, IPersistentProfile
where TLayoutEnum: struct, Enum
{
    public bool IsOpen => EditorControl.IsInsideTree;
    private TProfileEditor? _profileEditor;
    public TEditorControl EditorControl => ProfileEditor.EditorControl;
    public TProfileEditor ProfileEditor
    {
        get
        {
            if (_profileEditor != null)
                return _profileEditor;
            _profileEditor = new();
            _profileEditor.FinishSetup(EditorControl);
            return _profileEditor;
        }
    }

    public void Open()
    {
        ProfileEditor.Open();
    }

    public void Close()
    {
        ProfileEditor.Close();
    }
}