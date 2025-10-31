// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.UI.Core;

/// <summary>
/// Does this control support raising UIEvents
/// </summary>
public interface IUIEventDispatcher
{
    protected void RaiseUIEvent<TEvent>(TEvent args) where TEvent : struct
    {
        UIEvents.RaiseEvent(args);
    }
    protected void RaiseUIEvent<TEvent>(ref TEvent args) where TEvent : struct
    {
        UIEvents.RaiseEvent(ref args);
    }
};

public interface IUIEventSubscriber
{
    protected HashSet<UIEventHandle> EventHandles { get; set; }

    protected void SubscribeUIEvent<TEvent>(WriteableUIEvent<TEvent> handler) where TEvent: struct
    {
        if (!EventHandles.Add(UIEvents.Subscribe(handler)))
            throw new InvalidOperationException($"Type:{GetType()} Tried to subscribe to event{typeof(TEvent)} twice!");
    }
    protected void SubscribeUIEvent<TEvent>(UIEvent<TEvent> handler)  where TEvent: struct
    {
        if (!EventHandles.Add(UIEvents.Subscribe(handler)))
            throw new InvalidOperationException($"Type:{GetType()} Tried to subscribe to event{typeof(TEvent)} twice!");
    }

    protected UIEventHandle GetHandleByEventType<TEvent>() where TEvent: struct
    {
        var type = typeof(TEvent);
        foreach (var handle in EventHandles)
            if (handle.EventType == type)
                return handle;
        return new UIEventHandle();
    }

    protected bool TryGetHandleByEventType<TEvent>(out UIEventHandle foundHandle) where TEvent: struct
    {
        var type = typeof(TEvent);
        foreach (var handle in EventHandles)
            if (handle.EventType == type)
            {
                foundHandle = handle;
                return true;
            }

        foundHandle = new UIEventHandle();
        return false;
    }

    protected void RemoveHandleByType<TEvent>() where TEvent : struct
    {
        GetHandleByEventType<TEvent>().Unsubscribe();
    }

    protected void UnsubscribeUIEvents()
    {
        foreach (var handle in EventHandles)
            handle.Unsubscribe();
    }
}