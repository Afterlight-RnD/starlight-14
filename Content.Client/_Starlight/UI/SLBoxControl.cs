// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI;

[Virtual]
public class SLBox : BoxContainer, ISLControl
{
    public SLBox()
    {
    }

    [MustCallBase]
    protected override void EnteredTree()
    {
        this.RegisterUIEvents();
        UIEvents.RaiseControlEvent(this, new ControlEnteredTreeUIEvent());
    }

    public SLBox(LayoutOrientation orientation) => Orientation = orientation;

    [MustCallBase]
    protected override void ExitedTree()
    {
        UIEvents.RaiseControlEvent(this, new ControlExitedTreeUIEvent());
        this.UnsubscribeAllUIEvents();
    }
}