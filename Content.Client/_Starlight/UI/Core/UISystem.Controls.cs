// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

public abstract class BoundUISystem<TControl> : UISystem where TControl: Control, ISLControl,new()
{
    private List<TControl> _boundControls = new();

    [MustCallBase]
    public override void Initialize()
    {
        base.Initialize();
        UIEvents.Subscribe<TControl, ControlEnteredTreeUIEvent>(HandleBoundControlEnteredTree);
        UIEvents.Subscribe<TControl, ControlExitedTreeUIEvent>(HandleBoundControlExitedTree);
    }

    public IEnumerable<TControl> IterateBoundControl()
    {
        foreach (var control in _boundControls)
            yield return control;
    }

    private void HandleBoundControlEnteredTree(TControl control, ref readonly ControlEnteredTreeUIEvent args)
    {
        _boundControls.Add(control);
        BoundControlEnteredTree(control);
    }

    private void HandleBoundControlExitedTree(TControl control, ref readonly ControlExitedTreeUIEvent args)
    {
        _boundControls.Remove(control);
        BoundControlExitedTree(control);
    }

    [MustCallBase(true)]
    protected abstract void BoundControlEnteredTree(TControl boundControl);
    [MustCallBase(true)]
    protected abstract void BoundControlExitedTree(TControl boundControl);
}