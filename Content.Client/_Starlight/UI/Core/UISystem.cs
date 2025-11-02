// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

public abstract class UISystem : EntitySystem
{
    [Dependency] protected readonly UIEventBus UIEvents = default!;

    private HashSet<UIEventHandle> _uiEventHandles = new();

    public void RaiseUIEvent<T>(T args) where T : struct
    {
       UIEvents.RaiseEvent(args);
    }

    public void RaiseUIWritableEvent<T>(ref T args) where T : struct
    {
        UIEvents.RaiseWritableEvent(ref args);
    }

    public void SubscribeWritableUIEvent<T>(WriteableUIEvent<T> uiEvent) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeWritable(uiEvent));
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
public abstract class UISystem<TControl> : UISystem where TControl: Control, new()
{
    private HashSet<TControl> _registeredInstances = new();

    public override void Initialize()
    {
        SubscribeUIEvent<RegisterControlUIEvent>(OnControlRegistered);
        SubscribeUIEvent<DeregisterControlUIEvent>(OnControlDeregistered);
    }

    [MustCallBase(true)]
    protected virtual void ControlRegistered(TControl control) {}

    [MustCallBase(true)]
    protected virtual void ControlDeregistered(TControl control) {}

    private void OnControlDeregistered(ref readonly DeregisterControlUIEvent args)
    {
        ControlDeregistered(args.Instance);
        _registeredInstances.Remove(args.Instance);
    }

    private void OnControlRegistered(ref readonly RegisterControlUIEvent args)
    {
        _registeredInstances.Add(args.NewInstance);
        ControlRegistered(args.NewInstance);
    }

    public IEnumerable<TControl> IterateInstances()
    {
        foreach (var instance in _registeredInstances)
            yield return instance;
    }

    public record struct RegisterControlUIEvent(TControl NewInstance);
    public record struct DeregisterControlUIEvent(TControl Instance);
}