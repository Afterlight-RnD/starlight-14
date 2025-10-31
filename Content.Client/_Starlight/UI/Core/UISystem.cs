// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.UI.Core;

public abstract class UISystem : EntitySystem
{
    [Dependency] protected readonly UIEventBus UIEvents = default!;

    private HashSet<UIEventHandle> _uiEventHandles = new();

    public void SubscribeUIEvent<T>(WriteableUIEvent<T> uiEvent) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(uiEvent));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> uiEvent) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(uiEvent));
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