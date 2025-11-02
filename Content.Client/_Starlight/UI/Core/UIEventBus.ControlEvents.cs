// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Robust.Client.UserInterface;
using Robust.Shared.Collections;

namespace Content.Client._Starlight.UI.Core;

public sealed partial class UIEventBus
{

    private readonly Dictionary<(Type ControlType,Type EventType), ControlSubscriptions> _controlSubscriptions = new();

    private bool TryGetControlSubscription<TEvent>(Type controlType,[NotNullWhen(true)] out ControlSubscriptions? subscriptions)
        where TEvent : struct
    {
        return _controlSubscriptions.TryGetValue((controlType, typeof(TEvent)), out subscriptions);
    }

    private bool TryGetControlSubscription<TControl,TEvent>([NotNullWhen(true)] out ControlSubscriptions<TControl,TEvent>? subscriptions)
        where TControl: Control, new()
        where TEvent : struct
    {
        subscriptions = null;
        if (!_controlSubscriptions.TryGetValue((typeof(TControl),typeof(TEvent)), out var rawSubscriptions))
            return false;
        subscriptions = rawSubscriptions.GetTyped<TControl,TEvent>();
        return true;
    }

    private ControlSubscriptions<TControl,TEvent> EnsureControlSubscription<TControl,TEvent>()
        where TControl: Control, new()
        where TEvent : struct
    {
        if (TryGetControlSubscription(out ControlSubscriptions<TControl,TEvent>? subs))
            return subs;
        subs = _typeFactory.CreateInstance<ControlSubscriptions<TControl,TEvent>>(true, false);
        _controlSubscriptions.Add((typeof(TControl),typeof(TEvent)), subs);
        return subs;
    }

     private abstract class ControlSubscriptions
    {
        public ControlSubscriptions<TControl,TEvent> GetTyped<TControl,TEvent>()
        where TControl: Control, new()
            where TEvent : struct
            => (ControlSubscriptions<TControl,TEvent>)this;

        public abstract void Raise<TEvent>(Control control, in TEvent args) where TEvent : struct;
        public abstract void RaiseWritable<TEvent>(Control control, ref TEvent args) where TEvent : struct;
        public abstract void Unsubscribe(ref UIEventHandle handle);
    };

    private sealed class ControlSubscriptions<TControl,TEvent> : ControlSubscriptions
        where TControl: Control, new()
        where TEvent : struct
    {
        private ValueList<(WriteableUIEvent<TControl,TEvent> handler, UIEventHandle handle)> Handlers = new();
        private ValueList<(UIEvent<TControl,TEvent>handler, UIEventHandle handle)> ReadonlyHandlers = new();
        private Dictionary<UIEventHandle, (bool readOnly, int idx)> _handleLookup = new();

        public UIEventHandle RegisterHandler(UIEventHandle handle, WriteableUIEvent<TControl,TEvent> del)
        {
            _handleLookup.Add(handle, (false, Handlers.Count));
            Handlers.Add((del, handle));
            return handle;
        }

        public UIEventHandle RegisterHandler(UIEventHandle handle, UIEvent<TControl,TEvent> del)
        {
            _handleLookup.Add(handle, (true, Handlers.Count));
            ReadonlyHandlers.Add((del, handle));
            return handle;
        }

        public override void Raise<TEvent1>(Control control, in TEvent1 args)
        {
            if (control is not TControl typedControl)
                return;
            if (args is not TEvent typedArgs)
                throw new InvalidOperationException($"EventTypeMismatch! Expected: {typeof(TEvent)} Got:{typeof(TEvent1)}");
            foreach (var (handler,_) in ReadonlyHandlers)
                handler.Invoke(typedControl, ref typedArgs);
        }

        public override void RaiseWritable<TEvent1>(Control control, ref TEvent1 args)
        {
            if (control is not TControl typedControl)
                return;
            if (args is not TEvent typedArgs)
                throw new InvalidOperationException($"EventTypeMismatch! Expected: {typeof(TEvent)} Got:{typeof(TEvent1)}");
            foreach (var (handler,_) in Handlers)
                handler.Invoke(typedControl, ref typedArgs);
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