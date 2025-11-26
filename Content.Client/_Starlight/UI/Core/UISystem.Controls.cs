// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

public abstract class BoundUISystem<TControl> : UISystem where TControl: Control, ISLControl,new()
{
    [MustCallBase]
    public override void Initialize()
    {
        base.Initialize();
        UIEvents.Subscribe<TControl, ControlEnteredTreeUIEvent>(HandleBoundControlEnteredTree);
        UIEvents.Subscribe<TControl, ControlExitedTreeUIEvent>(HandleBoundControlExitedTree);
    }

    private void HandleBoundControlEnteredTree(TControl control, ref readonly ControlEnteredTreeUIEvent args)
    {
        BoundControlEnteredTree(control);
    }

    private void HandleBoundControlExitedTree(TControl control, ref readonly ControlExitedTreeUIEvent args)
    {
        BoundControlExitedTree(control);
    }

    [MustCallBase(true)]
    protected abstract void BoundControlEnteredTree(TControl boundControl);
    [MustCallBase(true)]
    protected abstract void BoundControlExitedTree(TControl boundControl);
}