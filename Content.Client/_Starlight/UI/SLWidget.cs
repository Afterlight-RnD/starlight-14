// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI;

public abstract class SLWidget : UIWidget, ISLControl
{
    [Dependency] protected readonly IEntityManager EntityManager = default!;
    [MustCallBase]
    protected override void EnteredTree()
    {
        this.RegisterUIEvents();
        UIEvents.RaiseControlEvent(this,new ControlEnteredTreeUIEvent());
    }

    protected SLWidget()
    {
        IoCManager.InjectDependencies(this);
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        UIEvents.RaiseControlEvent(this, new ControlExitedTreeUIEvent());
        this.UnsubscribeAllUIEvents();
    }
}