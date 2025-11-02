// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Controls;

public abstract class SLWidget : UIWidget
{
    [Dependency] protected readonly IEntityManager EntityManager = default!;
    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    protected SLWidget()
    {
        IoCManager.InjectDependencies(this);
    }
    public void SubscribeWriteableUIEvent<T>(WriteableUIEvent<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeWriteable(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseUIEvent<T>(ref T args) where T : struct
    {
        UIEvents.RaiseWriteableEvent(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }
        _uiEventHandles.Clear();
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        UnsubscribeAllUIEvents();
    }
}


public abstract class SLWidget<TSelf,TSystem> : SLWidget
    where TSystem: UISystem<TSelf>
    where TSelf:SLWidget<TSelf,TSystem>, new()
{
    [MustCallBase(true)]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new UISystem<TSelf>.RegisterControlUIEvent((TSelf)this));
    }

    [MustCallBase(true)]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new UISystem<TSelf>.DeregisterControlUIEvent((TSelf)this));
        base.ExitedTree();
    }
}