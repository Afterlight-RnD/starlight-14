// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Content.Shared._Starlight.Body.Components;
using Content.Shared._Starlight.Body.Events;

namespace Content.Shared._Starlight.Body.Systems;

public sealed partial class BodySystem
{
    public bool SubscribeBodyEvent<TEvent>(IEntityEventSubscriber subscriber, BodyPartEventHandler<TEvent> handler)
        where TEvent: notnull => EnsureEventSubs<TEvent>().Subscribe(subscriber, handler);

    public bool SubscribeBodyEvent<TEvent>(IEntityEventSubscriber subscriber, BodyPartRefEventHandler<TEvent> handler)
        where TEvent: notnull => EnsureEventSubs<TEvent>().Subscribe(subscriber, handler);

    public bool UnSubscribeBodyEvent<TEvent>(IEntityEventSubscriber subscriber)
        where TEvent : notnull
    {
        var success = false;
        if (!TryGetEventSubs<TEvent>(out var subs))
            return success;
        if (subs.RegisteredListeners.Remove(subscriber, out var removed))
        {
            subs.Handlers -= removed;
            success = true;
        }
        if (!subs.RegisteredRefListeners.Remove(subscriber, out var removedRef)) return success;
        subs.RefHandlers -= removedRef;
        success = true;
        return success;
    }
    private Dictionary<Type, ISubs> _bodyEventRelays = new();

    private bool TryGetEventSubs<TEvent>([NotNullWhen(true)]out Subs<TEvent>? subs) where TEvent : notnull
    {
        var subType = typeof(TEvent);
        subs = null;
        if (!_bodyEventRelays.TryGetValue(subType, out var rawSubs)) return false;
        subs = (Subs<TEvent>)rawSubs;
        return true;
    }

    private Subs<TEvent> EnsureEventSubs<TEvent>() where TEvent : notnull
    {
        var subType = typeof(TEvent);
        if (!_bodyEventRelays.TryGetValue(subType, out var rawSubs))
        {
            var subs = new Subs<TEvent>(EntityManager);
            _bodyEventRelays.Add(subType, subs);
            return subs;
        }
       return (Subs<TEvent>)rawSubs;
    }


    private interface ISubs: IEntityEventSubscriber
    {
        IEventBus EventBus { get; }
        IEntityManager EntMan { get; }

        void UnsubscribeRelays();
    }

    private sealed class Subs<TEvent> : ISubs
        where TEvent : notnull
    {
        public Subs(IEntityManager entMan)
        {
            EventBus = entMan.EventBus;
            EntMan = entMan;
            EventBus.SubscribeLocalEvent<SLBodyComponent, TEvent>(RelayEvent);
            EventBus.SubscribeLocalEvent<SLBodyComponent, TEvent>(RelayRefEvent);
        }

        private void RelayEvent(EntityUid bodyId, SLBodyComponent bodyComp, TEvent args)
        {
            Entity<SLBodyComponent> body = (bodyId, bodyComp);
            foreach (var bodyPart in bodyComp.CachedBodyParts)
            {
                RelayToBodyEvents(body, bodyPart, args);
            }
        }

        private void RelayRefEvent(EntityUid bodyId, SLBodyComponent bodyComp, ref TEvent args)
        {
            Entity<SLBodyComponent> body = (bodyId, bodyComp);
            foreach (var bodyPart in bodyComp.CachedBodyParts)
            {
                RelayToBodyEvents(body, bodyPart, ref args);
            }
        }
        public void UnsubscribeRelays()
        {
            EventBus.UnsubscribeLocalEvent<SLBodyComponent,TEvent>();
        }

        public bool Subscribe(IEntityEventSubscriber subscriber,
            BodyPartEventHandler<TEvent> handler)
        {
            var self = this;
            if (!self.RegisteredListeners.TryAdd(subscriber, handler))
                return false;
            self.Handlers += handler;
            return true;
        }

        public bool Subscribe(IEntityEventSubscriber subscriber,
            BodyPartRefEventHandler<TEvent> handler)
        {
            var self = this;
            if (!self.RegisteredRefListeners.TryAdd(subscriber, handler))
                return false;
            self.RefHandlers += handler;
            return true;
        }

        public IEventBus EventBus { get; }
        public IEntityManager EntMan { get; }
        public event BodyPartEventHandler<TEvent>? Handlers;
        public event BodyPartRefEventHandler<TEvent>? RefHandlers;
        public Dictionary<IEntityEventSubscriber, BodyPartEventHandler<TEvent>> RegisteredListeners { get; } = new();
        public Dictionary<IEntityEventSubscriber,BodyPartRefEventHandler<TEvent>> RegisteredRefListeners { get; } = new();

        public void RelayToBodyEvents(Entity<SLBodyComponent> body, Entity<SLBodyPartComponent> bodyPart, TEvent args)
            => Handlers?.Invoke(body, bodyPart, args);

        public void RelayToBodyEvents(Entity<SLBodyComponent> body, Entity<SLBodyPartComponent> bodyPart, ref TEvent args)
            => RefHandlers?.Invoke(body, bodyPart, ref args);
    }

    private void CleanUpListeners()
    {

    }

    private void CleanUpRelaySubscriptions()
    {
        foreach (var (_, relay) in _bodyEventRelays)
        {
          relay.UnsubscribeRelays();
        }
    }
}
