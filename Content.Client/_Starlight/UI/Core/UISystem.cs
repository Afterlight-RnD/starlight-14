// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.UI.Core;

public abstract class UISystem : EntitySystem
{
    [Dependency] protected readonly UIEventBus UIEvents = default!;

    private Dictionary<Type,UIEventHandle> _uiEventHandles = new();

    public void SubscribeUIEvent<T>(WriteableUIEvent<T> uiEvent) where T : struct
    {
        _uiEventHandles.Add(typeof(T),UIEvents.Subscribe(uiEvent));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> uiEvent) where T : struct
    {
        _uiEventHandles.Add(typeof(T),UIEvents.Subscribe(uiEvent));
    }

    public void UnsubscribeUIEvent<T>() where T : struct
    {
        if (!_uiEventHandles.Remove(typeof(T), out var handle))
            return;
        UIEvents.Unsubscribe<T>(ref handle);
    }

    public override void Shutdown()
    {
        foreach (var (handleType, handle) in _uiEventHandles)
            UIEvents.Unsubscribe(handleType, handle);
        base.Shutdown();
    }
}