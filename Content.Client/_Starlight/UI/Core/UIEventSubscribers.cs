// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

/// <summary>
/// Handles automatic subscription/unsubscription for UIEvents
/// </summary>
/// <typeparam name="TSelf">Self Type</typeparam>
/// <typeparam name="TEvent">UIEvent To SubscribeTo</typeparam>
public interface IUIEventSubscriber<TSelf, TEvent> : ISLControl
    where TSelf : Control, IUIEventSubscriber<TSelf, TEvent>
    where TEvent : struct
{
    static IUIEventSubscriber()
    {
        var selfType = typeof(TSelf);
        if (!UIEventControlTypeRegistry._subscriberDelegates.TryGetValue(selfType, out var subTypes))
        {
            subTypes = new();
            UIEventControlTypeRegistry._subscriberDelegates.Add(selfType, subTypes);
        }

        subTypes.Add(static args =>
        {
            var self = (TSelf)args;
            self.SubscribeUIEvent<TSelf, TEvent>(self.HandleUIEvent);
        });
    }
    public void HandleUIEvent(ref readonly TEvent args);
}

/// <summary>
/// Handles automatic subscription/unsubscription for UIRequests
/// </summary>
/// <typeparam name="TSelf">Self Type</typeparam>
/// <typeparam name="TEvent">UIEvent To SubscribeTo</typeparam>
public interface IUIRequestSubscriber<TSelf, TEvent> : ISLControl
    where TSelf : Control, IUIRequestSubscriber<TSelf, TEvent>
    where TEvent : struct
{
    static IUIRequestSubscriber()
    {
        var selfType = typeof(TSelf);
        if (!UIEventControlTypeRegistry._subscriberDelegates.TryGetValue(selfType, out var subTypes))
        {
            subTypes = new();
            UIEventControlTypeRegistry._subscriberDelegates.Add(selfType, subTypes);
        }

        subTypes.Add(static args =>
        {
            var self = (TSelf)args;
            self.SubscribeUIRequest<TSelf, TEvent>(self.HandleUIRequest);
        });
    }
    public void HandleUIRequest(ref TEvent args);
}

public static class UIEventControlTypeRegistry
{
    public static Dictionary<Type, List<Action<Control>>> _subscriberDelegates = new();

    public static void RegisterUIEvents(Control control)
    {
        if (!_subscriberDelegates.TryGetValue(control.GetType(), out var regDelegates)) return;
        foreach (var del in regDelegates)
            del.Invoke(control);
    }
}