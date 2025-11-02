// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.Contracts;
using System.Threading;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

/// <summary>
/// Static to prevent needing to inject UIEventBus into controls via dep-injection
/// If this gets upstreamed into the engine, just add UIEventbus to UserInterfaceManager and remove this
/// </summary>
public static class UIEvents
{
    private static readonly ThreadLocal<UIEventBus> _container = new();

    private static UIEventBus LocalEventBus =>
         _container.Value ?? (_container.Value = IoCManager.Resolve<UIEventBus>());

    [Pure]
    public static UIEventHandle SubscribeWriteable<T>(WriteableUIEvent<T> handler) where T: struct
    {
        return LocalEventBus.SubscribeWritable(handler);
    }

    [Pure]
    public static UIEventHandle Subscribe<T>(UIEvent<T> handler) where T: struct
    {
        return LocalEventBus.Subscribe(handler);
    }

    public static void RaiseEvent<T>(in T args) where T : struct
    {
        LocalEventBus.RaiseEvent(in args);
    }

    public static void RaiseWriteableEvent<T>(ref T args) where T : struct
    {
        LocalEventBus.RaiseWritableEvent(ref args);
    }

    public static void Unsubscribe(ref UIEventHandle handle)
    {
        LocalEventBus.Unsubscribe(ref handle);
    }

    // == Control Events ==

    [Pure]
    public static UIEventHandle SubscribeWritable<TControl,TEvent>(WriteableUIEvent<TControl,TEvent> handler)
        where TControl: Control, new()
        where TEvent: struct
    {
        return  LocalEventBus.SubscribeWritable(handler);
    }

    [Pure]
    public static UIEventHandle Subscribe<TControl,TEvent>(UIEvent<TControl,TEvent> handler)
        where TControl: Control, new()
        where TEvent: struct
    {
       return  LocalEventBus.Subscribe(handler);
    }

    public static void RaiseControlEvent<TEvent>(Control control,in TEvent args)
        where TEvent : struct
    {
        LocalEventBus.RaiseControlEvent(control, in args);
    }

    public static void RaiseWritableControlEvent<TEvent>(Control control, ref TEvent args)
        where TEvent : struct
    {
        LocalEventBus.RaiseWritableControlEvent(control, ref args);
    }

    public static void RaiseControlEventRecursive<TEvent>(Control control, in TEvent args)
        where TEvent : struct
    {
        LocalEventBus.RaiseControlEventRecursive(control, in args);
    }

    public static void RaiseWritableControlEventRecursive<TEvent>(Control control, ref TEvent args)
        where TEvent : struct
    {
        LocalEventBus.RaiseWritableControlEventRecursive(control, ref args);
    }

}