// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Robust.Shared.Collections;

namespace Content.Client._Starlight.UI.Core;

public delegate void WriteableUIEvent<TEvent>(ref TEvent args) where TEvent: struct;

public delegate void UIEvent<TEvent>(ref readonly TEvent args) where TEvent: struct;

public sealed class UIEventBus
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;

    private readonly Dictionary<Type, Subscriptions> _broadcastSubscriptions = new();

    private bool TryGetSubscription<T>([NotNullWhen(true)] out Subscriptions<T>? subscriptions) where T : struct
    {
        subscriptions = null;
        if (!_broadcastSubscriptions.TryGetValue(typeof(T), out var rawSubscriptions))
            return false;
        subscriptions = rawSubscriptions.GetTyped<T>();
        return true;
    }

    private Subscriptions<T> EnsureSubscription<T>() where T : struct
    {
        if (TryGetSubscription(out Subscriptions<T>? subs))
            return subs;
        subs = _typeFactory.CreateInstance<Subscriptions<T>>(true,false);
        _broadcastSubscriptions.Add(typeof(T),subs);
        return subs;
    }

    [Pure]
    public UIEventHandle Subscribe<T>(WriteableUIEvent<T> handler) where T: struct
    {
        var subs = EnsureSubscription<T>();
        return subs.RegisterHandler(handler);
    }

    [Pure]
    public UIEventHandle Subscribe<T>(UIEvent<T> handler) where T: struct
    {
        var subs = EnsureSubscription<T>();
        return subs.RegisterHandler(handler);
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

    public void Unsubscribe<T>(ref UIEventHandle handle) where T: struct
    {
        if (!TryGetSubscription<T>(out var foundSubs))
            return;
        foundSubs.Unsubscribe(ref handle);
    }

    public void Unsubscribe(Type type, UIEventHandle handle)
    {
        if (!_broadcastSubscriptions.TryGetValue(type, out var foundSubs))
            return;
        foundSubs.Unsubscribe(ref handle);
    }

    public abstract class Subscriptions
    {
        public Subscriptions<TEvent> GetTyped<TEvent>() where TEvent : struct => (Subscriptions<TEvent>)this;

        public abstract void Unsubscribe(ref UIEventHandle handle);
    };

    public sealed class Subscriptions<TEvent> : Subscriptions where TEvent : struct
    {
        private ValueList<(WriteableUIEvent<TEvent> handler, UIEventHandle handle)> Handlers = new();
        private ValueList<(UIEvent<TEvent>handler, UIEventHandle handle)> ReadonlyHandlers = new();

        private int _nextHandle = 0;
        private Queue<UIEventHandle> _freeHandles = new();
        private Dictionary<UIEventHandle, (bool readOnly, int idx)> _handleLookup = new();

        public void Raise(TEvent args)
        {
            foreach (var (handler,_) in ReadonlyHandlers)
                handler.Invoke(ref args);
        }

        public void RaiseRef(ref TEvent args)
        {
            foreach (var (handler,_) in Handlers)
                handler.Invoke(ref args);
        }

        public UIEventHandle RegisterHandler(WriteableUIEvent<TEvent> del)
        {
            var handle = GetNextHandle();
            _handleLookup.Add(handle, (false, Handlers.Count));
            Handlers.Add((del, handle));

            return handle;
        }

        public UIEventHandle RegisterHandler(UIEvent<TEvent> del)
        {
            var handle = GetNextHandle();
            _handleLookup.Add(handle, (true, Handlers.Count));
            ReadonlyHandlers.Add((del, handle));
            return handle;
        }

        public override void Unsubscribe(ref UIEventHandle handle)
        {
            if (!handle.IsValid || !_handleLookup.Remove(handle, out var handlerData)) return;
            if (handlerData.readOnly)
            {
                var oldHandler = ReadonlyHandlers[^1];
                if (oldHandler.handle.Generation != handle.Generation)
                    return;
                ReadonlyHandlers[handlerData.idx] = oldHandler;
                _handleLookup[oldHandler.handle] = (true, handlerData.idx);
            }
            else
            {
                var oldHandler = Handlers[^1];
                if (oldHandler.handle.Generation != handle.Generation)
                    return;
                Handlers[handlerData.idx] = oldHandler;
                _handleLookup[oldHandler.handle] = (false, handlerData.idx);
            }
            FreeHandle(ref handle);
        }


        private void FreeHandle(ref UIEventHandle handle)
        {
            _freeHandles.Enqueue(new UIEventHandle(handle.Id, handle.Generation + 1, this));
            _handleLookup.Remove(handle);
            handle.Invalidate();
        }

        private UIEventHandle GetNextHandle()
        {
            if (_freeHandles.TryDequeue(out var handle))
                return handle;
            handle = new UIEventHandle(_nextHandle, 1, this);
            _nextHandle++;
            return handle;
        }
    }
}

public struct UIEventHandle : IDisposable, IEquatable<UIEventHandle>
{
    public bool IsValid => Generation != 0;

    public int Id { get; private set; }= 0;

    public int Generation { get; private set; } = 0;

    private UIEventBus.Subscriptions _subscriptions;

    [Access(typeof(UIEventBus))]
    public UIEventHandle(int id, int generation, UIEventBus.Subscriptions subscriptions)
    {
        Id = id;
        Generation = generation;
        _subscriptions = subscriptions;
    }

    public void Invalidate()
    {
        Generation = 0;
    }


    public void Dispose()
    {
        _subscriptions.Unsubscribe(ref this);
    }

    public bool Equals(UIEventHandle other) => _subscriptions.Equals(other._subscriptions) && Id == other.Id && Generation == other.Generation;

    public override bool Equals(object? obj) => obj is UIEventHandle other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(_subscriptions, Id, Generation);

    public static bool operator ==(UIEventHandle left, UIEventHandle right) => left.Equals(right);

    public static bool operator !=(UIEventHandle left, UIEventHandle right) => !left.Equals(right);
}