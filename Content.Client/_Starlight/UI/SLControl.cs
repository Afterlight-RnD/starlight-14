// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI;

[Virtual]
public  class SLControl : Control, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        this.RegisterUIEvents();
        UIEvents.RaiseControlEvent(this,new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        UIEvents.RaiseControlEvent(this,new ControlExitedTreeUIEvent());
        this.UnsubscribeAllUIEvents();
    }
}