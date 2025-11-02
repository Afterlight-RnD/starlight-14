// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Robust.Client.UserInterface;
using Robust.Shared.Collections;

namespace Content.Client._Starlight.UI.Core;

public sealed partial class UIEventBus: IPostInjectInit
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;
    [Dependency] private readonly IEntitySystemManager _systemManager = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;

    private readonly Dictionary<Type, Subscriptions> _subscriptions = new();

    private bool TryGetSubscription<TEvent>([NotNullWhen(true)] out Subscriptions<TEvent>? subscriptions) where TEvent : struct
    {
        subscriptions = null;
        if (!_subscriptions.TryGetValue(typeof(TEvent), out var rawSubscriptions))
            return false;
        subscriptions = rawSubscriptions.GetTyped<TEvent>();
        return true;
    }

    private Subscriptions<TEvent> EnsureSubscription<TEvent>() where TEvent : struct
    {
        if (TryGetSubscription(out Subscriptions<TEvent>? subs))
            return subs;
        subs = _typeFactory.CreateInstance<Subscriptions<TEvent>>(true, false);
        _subscriptions.Add(typeof(TEvent), subs);
        return subs;
    }

    private int _nextHandle = 0;
    private readonly Queue<UIEventHandle> _freeHandles = new();

    private void FreeHandle(ref UIEventHandle handle)
    {
        _freeHandles.Enqueue(new UIEventHandle(handle.Id, handle.Generation + 1, this, handle.EventType,
            handle.ControlType));
        handle.Unsubscribe();
    }

    private UIEventHandle GetNextHandle(Type handleType, Type? controlType)
    {
        if (_freeHandles.TryDequeue(out var handle))
            return new UIEventHandle(handle.Id, handle.Generation, this, handleType, controlType);

        handle = new UIEventHandle(_nextHandle, 1, this, handleType, controlType);
        _nextHandle++;
        return handle;
    }

    private abstract class Subscriptions
    {
        public Subscriptions<TEvent> GetTyped<TEvent>() where TEvent : struct => (Subscriptions<TEvent>)this;

        public abstract void Unsubscribe(ref UIEventHandle handle);
    };

    private sealed class Subscriptions<TEvent> : Subscriptions where TEvent : struct
    {
        private ValueList<(WriteableUIEvent<TEvent> handler, UIEventHandle handle)> Handlers = new();
        private ValueList<(UIEvent<TEvent>handler, UIEventHandle handle)> ReadonlyHandlers = new();
        private Dictionary<UIEventHandle, (bool readOnly, int idx)> _handleLookup = new();


        public void Raise(in TEvent args)
        {
            foreach (var (handler,_) in ReadonlyHandlers)
                handler.Invoke(in args);
        }

        public void RaiseWritable(ref TEvent args)
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

    public void PostInject()
    {
        _uiManager.OnScreenChanged += OnUIScreenChanged;
    }
}