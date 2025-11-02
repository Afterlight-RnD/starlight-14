// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI;

[Virtual]
public  class SLControl : Control
{
    #region UIEvents
    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();
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
    #endregion
}