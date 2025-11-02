// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.Contracts;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

public delegate void WriteableUIEvent<TEvent>(ref TEvent args) where TEvent: struct;

public delegate void UIEvent<TEvent>(ref readonly TEvent args) where TEvent: struct;

public delegate void WriteableUIEvent<in TControl, TEvent>(TControl control, ref TEvent args)
    where TControl : Control, new()
    where TEvent: struct;

public delegate void UIEvent<in TControl,TEvent>(TControl control, ref readonly TEvent args)
    where TControl : Control, new()
    where TEvent: struct;

public sealed partial class UIEventBus
{
    //== Global UI Events ==

    [Pure]
    public UIEventHandle SubscribeWritable<TEvent>(WriteableUIEvent<TEvent> handler) where TEvent: struct
    {
        var subs = EnsureSubscription<TEvent>();
        return subs.RegisterHandler(GetNextHandle(typeof(TEvent), null),handler);
    }

    [Pure]
    public UIEventHandle Subscribe<TEvent>(UIEvent<TEvent> handler) where TEvent: struct
    {
        var subs = EnsureSubscription<TEvent>();
        return subs.RegisterHandler(GetNextHandle(typeof(TEvent), null), handler);
    }
    public void RaiseEvent<TEvent>(in TEvent args) where TEvent : struct
    {
        if (!TryGetSubscription<TEvent>(out var foundSubs))
            return;
        foundSubs.Raise(args);
    }

    public void RaiseWritableEvent<TEvent>(ref TEvent args) where TEvent : struct
    {
        if (!TryGetSubscription<TEvent>(out var foundSubs))
            return;
        foundSubs.RaiseWritable(ref args);
    }

    //==Control UiEvents==

    [Pure]
    public UIEventHandle SubscribeWritable<TControl,TEvent>(WriteableUIEvent<TControl,TEvent> handler)
        where TControl: Control, new()
        where TEvent: struct
    {
        var subs = EnsureControlSubscription<TControl,TEvent>();
        return subs.RegisterHandler(GetNextHandle(typeof(TEvent), typeof(TControl)),handler);
    }

    [Pure]
    public UIEventHandle Subscribe<TControl,TEvent>(UIEvent<TControl,TEvent> handler)
        where TControl: Control, new()
        where TEvent: struct
    {
        var subs = EnsureControlSubscription<TControl,TEvent>();
        return subs.RegisterHandler(GetNextHandle(typeof(TEvent), typeof(TControl)), handler);
    }

    public void RaiseControlEvent<TEvent>(Control control,in TEvent args)
        where TEvent : struct
    {
        if (!TryGetControlSubscription<TEvent>(control.GetType(),out var foundSubs))
            return;
        foundSubs.Raise(control, in args);
    }

    public void RaiseWritableControlEvent<TEvent>(Control control, ref TEvent args)
        where TEvent : struct
    {
        if (!TryGetControlSubscription<TEvent>(control.GetType(),out var foundSubs))
            return;
        foundSubs.RaiseWritable(control, ref args);
    }

    public void RaiseControlEventRecursive<TEvent>(Control control, in TEvent args)
        where TEvent : struct
    {
        RaiseControlEvent(control,in args);
        foreach (var childControl in control.Children)
            RaiseControlEventRecursive(childControl,in args);
    }

    public void RaiseWritableControlEventRecursive<TEvent>(Control control, ref TEvent args)
        where TEvent : struct
    {
        RaiseWritableControlEvent(control,ref args);
        foreach (var childControl in control.Children)
            RaiseWritableControlEventRecursive(childControl,ref args);
    }

    //==Event Helpers for screens ==

    public void RaiseScreenEvent<TEvent>(UIScreen screen, in TEvent args)
        where TEvent : struct
    {
        RaiseControlEventRecursive(screen,in args);
    }

    public void RaiseScreenWritableEvent<TEvent>(UIScreen screen, ref TEvent args)
        where TEvent : struct
    {
        RaiseWritableControlEventRecursive(screen,ref args);
    }

    //==Common==

    public void Unsubscribe(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _subscriptions.TryGetValue(handle.EventType, out var subs))
            return;
        subs?.Unsubscribe(ref handle);
        FreeHandle(ref handle);
    }
}

public struct UIEventHandle : IEquatable<UIEventHandle>
{
    public bool IsValid => Generation != 0 || EventType == null;

    public int Id { get; private set; }= 0;

    public int Generation { get; private set; } = 0;

    public UIEventBus Owner { get; private set; }

    public Type EventType { get; private set; }

    public Type? ControlType { get; private set; }

    [Access(typeof(UIEventBus))]
    public UIEventHandle(int id, int generation, UIEventBus owner, Type eventType, Type? controlType)
    {
        Id = id;
        Generation = generation;
        Owner = owner;
        EventType = eventType;
        ControlType = controlType;
    }

    public void Unsubscribe()
    {
        Owner.Unsubscribe(ref this);
        Generation = 0;
    }

    [Access(typeof(UIEventBus))]
    public void Invalidate()
    {
        Generation = 0;
    }

    public bool Equals(UIEventHandle other) => Owner.Equals(other.Owner)
                                               && Id == other.Id
                                               && Generation == other.Generation;

    public override bool Equals(object? obj) => obj is UIEventHandle other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Owner, Id, Generation);

    public static bool operator ==(UIEventHandle left, UIEventHandle right) => left.Equals(right);

    public static bool operator !=(UIEventHandle left, UIEventHandle right) => !left.Equals(right);
}