// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using Robust.Shared.Collections;

namespace Content.Client._Starlight.UI.Core;

public sealed partial class UIEventBus
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;

    private readonly Dictionary<Type, Subscriptions> _subscriptions = new();

    private bool TryGetSubscription<T>([NotNullWhen(true)] out Subscriptions<T>? subscriptions) where T : struct
    {
        subscriptions = null;
        if (!_subscriptions.TryGetValue(typeof(T), out var rawSubscriptions))
            return false;
        subscriptions = rawSubscriptions.GetTyped<T>();
        return true;
    }

    private Subscriptions<T> EnsureSubscription<T>() where T : struct
    {
        if (TryGetSubscription(out Subscriptions<T>? subs))
            return subs;
        subs = _typeFactory.CreateInstance<Subscriptions<T>>(true, false);
        _subscriptions.Add(typeof(T), subs);
        return subs;
    }

    private int _nextHandle = 0;
    private readonly Queue<UIEventHandle> _freeHandles = new();

    private void FreeHandle(ref UIEventHandle handle)
    {
        _freeHandles.Enqueue(new UIEventHandle(handle.Id, handle.Generation + 1, this, handle.EventType));
        handle.Unsubscribe();
    }

    private UIEventHandle GetNextHandle(Type handleType)
    {
        if (_freeHandles.TryDequeue(out var handle))
            return handle;
        handle = new UIEventHandle(_nextHandle, 1, this, handleType);
        _nextHandle++;
        return handle;
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

        public UIEventHandle RegisterHandler(UIEventHandle handle, WriteableUIEvent<TEvent> del)
        {
            _handleLookup.Add(handle, (false, Handlers.Count));
            Handlers.Add((del, handle));

            return handle;
        }

        public UIEventHandle RegisterHandler(UIEventHandle handle, UIEvent<TEvent> del)
        {
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
        }
    }
}