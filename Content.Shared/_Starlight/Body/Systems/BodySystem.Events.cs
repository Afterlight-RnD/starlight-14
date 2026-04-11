// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Content.Shared._Starlight.Body.Components;
using Content.Shared._Starlight.Body.Events;

namespace Content.Shared._Starlight.Body.Systems;

public sealed partial class BodySystem
{
    /// <summary>
    /// Subscribes to an entity event on a body *from* a BodyPart. Can only be called when Event subscriptions are unlocked such as from within EntitySystem.Initialize!
    /// </summary>
    /// <param name="subscriber">subscription owner</param>
    /// <param name="handler">event handler</param>
    /// <typeparam name="TEvent">event type</typeparam>
    /// <returns>returns true if successful, false if it already exists</returns>
    public bool SubscribeBodyEvent<TEvent>(IEntityEventSubscriber subscriber, BodyPartEventHandler<TEvent> handler)
        where TEvent : notnull => EnsureEventSubs<TEvent>().Subscribe(subscriber, handler);

    /// <summary>
    /// Subscribes to an entity event on a body *from* a BodyPart. Can only be called when Event subscriptions are unlocked such as from within EntitySystem.Initialize!
    /// </summary>
    /// <param name="subscriber">subscription owner</param>
    /// <param name="handler">event handler</param>
    /// <typeparam name="TEvent">event type</typeparam>
    /// <returns>returns true if successful, false if it already exists</returns>
    public bool SubscribeBodyEvent<TEvent>(IEntityEventSubscriber subscriber, BodyPartRefEventHandler<TEvent> handler)
        where TEvent : notnull => EnsureEventSubs<TEvent>().Subscribe(subscriber, handler);

    /// <summary>
    /// Unsubscribe a BodyPart from a body entity event. (Optionally) remove the relays if they are empty
    /// WARNING: relay events can only be added *once* per game lifetime and clearing them will remove them for the rest of the game lifecycle!
    /// </summary>
    /// <param name="subscriber"></param>
    /// <param name="unsubRelays"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public bool UnSubscribeBodyEvent<TEvent>(IEntityEventSubscriber subscriber, bool unsubRelays = false)
        where TEvent : notnull
    {
        var success = false;
        if (!TryGetEventRegistry<TEvent>(out var subs))
            return success;
        subs.ClearListener(subscriber, unsubRelays);
        return success;
    }

    #region Event Relay Implementation

    /// <summary>
    /// Tries to get event registry by type
    /// </summary>
    /// <param name="register">found register</param>
    /// <typeparam name="TEvent">event type</typeparam>
    /// <returns>true if successful</returns>
    private bool TryGetEventRegistry<TEvent>([NotNullWhen(true)] out SubRegisterRegister<TEvent>? register) where TEvent : notnull
    {
        var subType = typeof(TEvent);
        register = null;
        if (!_eventRelayRegistry.TryGetValue(subType, out var rawSubs)) return false;
        register = (SubRegisterRegister<TEvent>)rawSubs;
        return true;
    }

    /// <summary>
    /// Attempts to remove an event register. (Optionally) will clear relays as well.
    /// WARNING: relay events can only be added *once* per game lifetime and clearing them will remove them for the rest of the game lifecycle!
    /// </summary>
    /// <param name="clearRelays">should we clear relays? Warning: only can do once per game lifecycle!</param>
    /// <typeparam name="TEvent">event type</typeparam>
    /// <returns></returns>
    private bool RemoveEventRegister<TEvent>(bool clearRelays = false) where TEvent : notnull
    {
        var subType = typeof(TEvent);
        if (!_eventRelayRegistry.TryGetValue(subType, out var registry))
            return false;
        registry.ClearListeners(clearRelays);
        return true;
    }

    /// <summary>
    /// Ensure that a specific Event has a registry. Can only be called when Event subscriptions are unlocked such as from within EntitySystem.Initialize!
    /// </summary>
    /// <typeparam name="TEvent">event type</typeparam>
    /// <returns>created event sub</returns>
    private SubRegisterRegister<TEvent> EnsureEventSubs<TEvent>() where TEvent : notnull
    {
        var subType = typeof(TEvent);
        if (!_eventRelayRegistry.TryGetValue(subType, out var rawSubs))
        {
            var subs = new SubRegisterRegister<TEvent>(EntityManager);
            _eventRelayRegistry.Add(subType, subs);
            return subs;
        }

        return (SubRegisterRegister<TEvent>)rawSubs;
    }

    /// <summary>
    /// Registry of all body event subscription registers indexed by event type.
    /// Lazily populated during system initialization when body events are subscribed to
    /// </summary>
    private Dictionary<Type, ISubRegister> _eventRelayRegistry = new();

    /// <summary>
    /// A register of body event subscriptions, stores handler signatures and performs dispatch to listeners
    /// </summary>
    private interface ISubRegister : IEntityEventSubscriber
    {
        /// <summary>
        /// EventBus Accessor
        /// </summary>
        IEventBus EventBus { get; }

        /// <summary>
        /// EntityManager dependency
        /// </summary>
        IEntityManager EntMan { get; }

        /// <summary>
        /// Clears all relay events!
        /// WARNING: relay events can only be added *once* per game lifetime and clearing them will remove them for the rest of the game lifecycle!
        /// </summary>
        void ClearRelays();

        /// <summary>
        /// Clears an event subscriber's listeners
        /// WARNING: registries can only be added *once* per game lifetime and clearing them will remove them for the rest of the game lifecycle!
        /// </summary>
        /// <param name="clearRegistryIfEmpty">should we clear event registries as well?</param>
        void ClearListener(IEntityEventSubscriber subscriber, bool clearRegistryIfEmpty = false);

        /// <summary>
        /// Clears all listeners and (optionally) relay events.
        /// WARNING: relay events can only be added *once* per game lifetime and clearing them will remove them for the rest of the game lifecycle!
        /// </summary>
        /// <param name="clearRelays">should we clear relay events as well?</param>
        void ClearListeners(bool clearRelays = false);

        /// <summary>
        /// Clear all data, use for cleanup ONLY
        /// </summary>
        void ClearAll();

        /// <summary>
        /// Do we have any listeners registered?
        /// </summary>
        bool HasListeners { get; }
    }

    /// <summary>
    /// Typed register, these are created at initialization and share lifecycle with bodySystem.
    /// These are created *once* lazily when BodyEvents are subscribed/registered.
    /// </summary>
    /// <typeparam name="TEvent">The event being relayed</typeparam>
    private sealed class SubRegisterRegister<TEvent> : ISubRegister
        where TEvent : notnull
    {
        public IEventBus EventBus { get; }
        public IEntityManager EntMan { get; }
        public event BodyPartEventHandler<TEvent>? Handlers;
        public event BodyPartRefEventHandler<TEvent>? RefHandlers;

        public event BodyPartEventCondition<TEvent>? ConditionalHandlers;
        public event BodyPartRefEventCondition<TEvent>? ConditionalRefHandlers;
        public Dictionary<IEntityEventSubscriber, BodyPartEventHandler<TEvent>> RegisteredListeners { get; } = new();

        public Dictionary<IEntityEventSubscriber, BodyPartRefEventHandler<TEvent>> RegisteredRefListeners { get; } =
            new();

        public Dictionary<IEntityEventSubscriber, BodyPartEventCondition<TEvent>> RegisteredConditionalListeners { get; } = new();

        public Dictionary<IEntityEventSubscriber, BodyPartRefEventCondition<TEvent>> RegisteredConditionalRefListeners { get; } =
            new();


        public bool HasListeners => Handlers != null && RefHandlers != null && ConditionalHandlers != null && ConditionalRefHandlers != null;

        public SubRegisterRegister(IEntityManager entMan)
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

        public void ClearRelays()
        {
            EventBus.UnsubscribeLocalEvent<SLBodyComponent, TEvent>();
        }

        public void ClearListener(IEntityEventSubscriber subscriber, bool clearRegistryIfEmpty = false)
        {
            if (RegisteredListeners.Remove(subscriber, out var handler))
                Handlers -= handler;
            if (RegisteredRefListeners.Remove(subscriber, out var refHandler))
                RefHandlers -= refHandler;
            if (clearRegistryIfEmpty && !HasListeners)
                ClearRelays();
        }

        public void ClearListeners(bool clearRelays = false)
        {
            RegisteredListeners.Clear();
            RegisteredRefListeners.Clear();
            Handlers = null;
            RefHandlers = null;

            if (clearRelays)
                ClearRelays();
        }

        public void ClearAll()
        {
            ClearListeners(true);
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

        public void RelayToBodyEvents(Entity<SLBodyComponent> body, Entity<SLBodyPartComponent> bodyPart, TEvent args)
            => Handlers?.Invoke(body, bodyPart, args);

        public void RelayToBodyEvents(Entity<SLBodyComponent> body, Entity<SLBodyPartComponent> bodyPart,
            ref TEvent args)
            => RefHandlers?.Invoke(body, bodyPart, ref args);
    }

    #endregion
}
