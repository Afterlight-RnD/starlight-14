// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.Contracts;

namespace Content.Client._Starlight.UI.Core;

public delegate void WriteableUIEvent<TEvent>(ref TEvent args) where TEvent: struct;

public delegate void UIEvent<TEvent>(ref readonly TEvent args) where TEvent: struct;

public sealed partial class UIEventBus
{
    [Pure]
    public UIEventHandle Subscribe<T>(WriteableUIEvent<T> handler) where T: struct
    {
        var subs = EnsureSubscription<T>();
        return subs.RegisterHandler(GetNextHandle(typeof(T)),handler);
    }

    [Pure]
    public UIEventHandle Subscribe<T>(UIEvent<T> handler) where T: struct
    {
        var subs = EnsureSubscription<T>();
        return subs.RegisterHandler(GetNextHandle(typeof(T)), handler);
    }

    public void RaiseEvent<T>(T args) where T : struct
    {
        if (!TryGetSubscription<T>(out var foundSubs))
            return;
        foundSubs.Raise(args);
    }

    public void RaiseEvent<T>(ref T args) where T : struct
    {
        if (!TryGetSubscription<T>(out var foundSubs))
            return;
        foundSubs.RaiseRef(ref args);
    }

    public void Unsubscribe(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _subscriptions.TryGetValue(handle.EventType!, out var subs))
            return;
        subs?.Unsubscribe(ref handle);
        FreeHandle(ref handle);
    }
}

public struct UIEventHandle : IDisposable, IEquatable<UIEventHandle>
{
    public bool IsValid => Generation != 0 || EventType == null;

    public int Id { get; private set; }= 0;

    public int Generation { get; private set; } = 0;

    public UIEventBus Owner { get; private set; }

    public Type EventType { get; private set; }

    [Access(typeof(UIEventBus))]
    public UIEventHandle(int id, int generation, UIEventBus owner, Type eventType)
    {
        Id = id;
        Generation = generation;
        Owner = owner;
        EventType = eventType;
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


    public void Dispose()
    {
        Unsubscribe();
    }

    public bool Equals(UIEventHandle other) => Owner.Equals(other.Owner) && Id == other.Id && Generation == other.Generation;

    public override bool Equals(object? obj) => obj is UIEventHandle other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Owner, Id, Generation);

    public static bool operator ==(UIEventHandle left, UIEventHandle right) => left.Equals(right);

    public static bool operator !=(UIEventHandle left, UIEventHandle right) => !left.Equals(right);
}