// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI;

public abstract class SLWidget : UIWidget, ISLControl
{
    [Dependency] protected readonly IEntityManager EntityManager = default!;
    [MustCallBase]
    protected override void EnteredTree()
    {
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
        UnsubscribeAllUIEvents();
    }

    #region UIEvents
    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();
    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseControlUIEvent<T>(T args)
        where T : struct
    {
        UIEvents.RaiseControlEvent(this,args);
    }

    public void RaiseControlUIEvent<T>(ref T args) where T : struct
    {
        UIEvents.RaiseControlRequest(this,ref args);
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
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
    #endregion
}