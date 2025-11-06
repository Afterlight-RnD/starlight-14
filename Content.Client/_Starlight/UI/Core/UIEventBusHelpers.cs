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
    private static UIEventBus LocalEventBus => IoCManager.Resolve<UIEventBus>();

    [Pure]
    public static UIEventHandle SubscribeRequest<T>(UIRequest<T> handler) where T: struct
    {
        return LocalEventBus.SubscribeRequest(handler);
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

    public static void RaiseRequest<T>(ref T args) where T : struct
    {
        LocalEventBus.RaiseRequest(ref args);
    }

    public static void Unsubscribe(ref UIEventHandle handle)
    {
        LocalEventBus.Unsubscribe(ref handle);
    }

    // == Control Events ==

    [Pure]
    public static UIEventHandle SubscribeRequest<TControl,TEvent>(UIRequest<TControl,TEvent> handler)
        where TControl: Control, new()
        where TEvent: struct
    {
        return  LocalEventBus.SubscribeRequest(handler);
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

    public static void RaiseControlRequest<TEvent>(Control control, ref TEvent args)
        where TEvent : struct
    {
        LocalEventBus.RaiseControlRequest(control, ref args);
    }

    public static void RaiseControlEventRecursive<TEvent>(Control control, in TEvent args)
        where TEvent : struct
    {
        LocalEventBus.RaiseControlEventRecursive(control, in args);
    }

    public static void RaiseControlRequestRecursive<TEvent>(Control control, ref TEvent args)
        where TEvent : struct
    {
        LocalEventBus.RaiseControlRequestRecursive(control, ref args);
    }

}