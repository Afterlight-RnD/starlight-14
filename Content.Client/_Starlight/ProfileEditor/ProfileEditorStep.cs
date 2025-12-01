// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor;

public interface IProfileEditorStep
{
    public void Enter(Control root);

    public void Exit(Control root);
}