// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

public abstract class UISystem : EntitySystem
{
    [Dependency] protected readonly UIEventBus UIEvents = default!;

    private HashSet<UIEventHandle> _uiEventHandles = new();

    public void RaiseUIEvent<TEvent>(TEvent args) where TEvent : struct
    {
       UIEvents.RaiseEvent(args);
    }

    public void RaiseUIRequest<TEvent>(ref TEvent args) where TEvent : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void SubscribeUIEvent<TEvent>(UIEvent<TEvent> uiEvent) where TEvent : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(uiEvent));
    }

    public void SubscribeUIEvent<TControl,TEvent>(UIEvent<TControl,TEvent> uiEvent)
        where TControl: Control, new()
        where TEvent : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(uiEvent));
    }

    public void SubscribeUIRequest<TEvent>(UIRequest<TEvent> uiRequest) where TEvent : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(uiRequest));
    }

    public void SubscribeUIRequest<TControl,TEvent>(UIRequest<TControl,TEvent> uiRequest)
        where TControl: Control, new()
        where TEvent : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(uiRequest));
    }

    public void UnSubscribeUIEvents()
    {
        foreach (var handle in _uiEventHandles)
            handle.Unsubscribe();
        _uiEventHandles.Clear();
    }

    public override void Shutdown()
    {
        UnSubscribeUIEvents();
        base.Shutdown();
    }
}