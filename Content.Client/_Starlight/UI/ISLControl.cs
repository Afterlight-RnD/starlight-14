// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI;


/// <summary>
/// Base interface for all starlight controls. Only used for generic type constraints
/// </summary>
public interface ISLControl
{
}

public static partial class SLControlExtensions
{
    private static Dictionary<Control, HashSet<UIEventHandle>> _controlSubscriptions = new();

    private static HashSet<UIEventHandle> EnsureControlHandles(Control control)
    {
        if (_controlSubscriptions.TryGetValue(control, out var handles))
            return handles;
        handles = new();
        _controlSubscriptions.Add(control, handles);
        return handles;
    }

    private static bool TryGetControlHandles(Control control, [NotNullWhen(true)] out HashSet<UIEventHandle>? handles)
    {
        return _controlSubscriptions.TryGetValue(control, out handles);
    }

    public static void RaiseControlUIEvent<TControl,T>(this TControl control, T args)
        where TControl: Control, new()
        where T : struct
    {
        UIEvents.RaiseControlEvent(control,args);
    }

    public static void RaiseControlUIEvent<TControl,T>(this TControl control,ref T args)
        where TControl: Control, new()
        where T : struct
    {
        UIEvents.RaiseControlRequest(control, ref args);
    }

    public static bool RegisterUIEventHandle<TControl>(this TControl control,UIEventHandle handle)
        where TControl: Control
    {
        return EnsureControlHandles(control).Add(handle);
    }

    public static void SubscribeUIRequest<TControl,T>(this TControl control,UIRequest<T> handler)
        where T : struct
        where TControl: Control
    {
        RegisterUIEventHandle(control,UIEvents.SubscribeRequest(handler));
    }

    public static void SubscribeUIEvent<TControl,T>(this TControl control,UIEvent<T> handler)
        where T : struct
        where TControl: Control
    {
        RegisterUIEventHandle(control, UIEvents.Subscribe(handler));
    }

    public static void RaiseUIEvent<T>(this Control control,T args)
        where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public static void RaiseRequest<T>(this Control control,ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public static void RegisterUIEvents(this Control control)
    {
        UIEventControlTypeRegistry.RegisterUIEvents(control);
    }

    public static void UnsubscribeUIEvent<TControl>(this TControl control,ref UIEventHandle handle)
        where TControl: Control
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || !TryGetControlHandles(control, out var handles)|| handles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public static void UnsubscribeAllUIEvents(Control control)

    {
        if (!TryGetControlHandles(control, out var handles))
            return;
        foreach (var handle in handles)
        {
            handle.Unsubscribe();
        }
        handles.Clear();
        _controlSubscriptions.Remove(control);
    }

    public static void UnsubscribeAllUIEvents<TControl>(this TControl control)
        where TControl: Control
    {
        if (!TryGetControlHandles(control, out var handles))
            return;
        foreach (var handle in handles)
        {
            handle.Unsubscribe();
        }
        handles.Clear();
        _controlSubscriptions.Remove(control);
    }


}