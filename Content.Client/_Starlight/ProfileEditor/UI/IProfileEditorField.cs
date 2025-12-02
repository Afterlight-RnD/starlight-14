// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;

public abstract class ProfileEditorField<TProfileData,TEditorControl,TProfileEditor> : SLBox
where TProfileData: IPersistentProfile, new()
where TEditorControl: SLControl, IProfileEditorMainControl<TEditorControl, TProfileEditor>
where TProfileEditor: IProfileEditor<TProfileEditor>, new()
{
    public static TEditorControl? FindParentEditor(Control control)
    {
        return control.FindParentOfTypeRecursive<TEditorControl>();
    }
}