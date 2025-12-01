// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;
using Content.Client._Starlight.UI;
using Content.Client._Starlight.UI.Core;

namespace Content.Client._Starlight.ProfileEditor;


public interface IProfileEditorSystem
{
    public void RegisterEditor(IProfileEditor instance);
}

public abstract class ProfileEditorSystem<TEditor, TEditorControl> : UISystem, IProfileEditorSystem
    where TEditor : class, IProfileEditor, new()
    where TEditorControl : ProfileEditorMainControl<TEditor>, ISLControl, new()
{
    private List<TEditor> _editors = new();

    void IProfileEditorSystem.RegisterEditor(IProfileEditor instance)
    {
        _editors.Add((TEditor)instance);
    }

    public IEnumerable<TEditor> IterateEditors(bool onlyActive = true)
    {
        if (!onlyActive)
        {
            foreach (var editor in _editors)
                yield return editor;
        }
        else
        {
            foreach (var editor in _editors)
                if (editor.IsOpen)
                    yield return editor;
        }
    }

}