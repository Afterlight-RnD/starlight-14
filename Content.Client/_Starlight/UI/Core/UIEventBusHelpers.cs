// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.Contracts;
using System.Threading;

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
    public static UIEventHandle Subscribe<T>(WriteableUIEvent<T> handler) where T: struct
    {
        return LocalEventBus.Subscribe(handler);
    }

    [Pure]
    public static UIEventHandle Subscribe<T>(UIEvent<T> handler) where T: struct
    {
        return LocalEventBus.Subscribe(handler);
    }

    public static void RaiseEvent<T>(T args) where T : struct
    {
        LocalEventBus.RaiseEvent(args);
    }

    public static void RaiseEvent<T>(ref T args) where T : struct
    {
        LocalEventBus.RaiseEvent(args);
    }

    public static void Unsubscribe(ref UIEventHandle handle)
    {
        LocalEventBus.Unsubscribe(ref handle);
    }
}