// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Controls;

public abstract class SLWidget : UIWidget, IUIEventDispatcher, IUIEventSubscriber
{
    protected HashSet<UIEventHandle> UIEventHandles { get; } = new();

    public void SubscribeWriteableUIEvent<T>(WriteableUIEvent<T> handler) where T: struct
    {
        UIEventHandles.Add(UIEvents.SubscribeWriteable(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T: struct
    {
        UIEventHandles.Add(UIEvents.Subscribe(handler));
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
        if (!handle.IsValid || UIEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in UIEventHandles)
        {
            handle.Unsubscribe();
        }
        UIEventHandles.Clear();
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        UnsubscribeAllUIEvents();
    }
}