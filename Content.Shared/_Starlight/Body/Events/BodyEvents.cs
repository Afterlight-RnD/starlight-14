// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Body.Components;
using Content.Shared._Starlight.Body.Systems;

namespace Content.Shared._Starlight.Body.Events;

public interface IBodyEvent
{
   [Access(typeof(BodySystem))]
   Entity<SLBodyComponent> Body { get; set; }
}

public interface ICancellableBodyEvent
{
    bool IsCancelled { get; set; }
}

public interface IBodyPartEvent
{
    [Access(typeof(BodySystem))]
    Entity<SLBodyPartComponent> BodyPart { get; set; }

    [Access(typeof(BodySystem))]
    Entity<SLBodyComponent> Body { get; set; }
}

public interface ICancellableBodyPartEvent : IBodyPartEvent, ICancellableBodyEvent;

public record struct PartAddedToBodyEvent(Entity<SLBodyPartComponent> Part);

public record struct PartRemovedFromBodyEvent(Entity<SLBodyPartComponent> Part);

public record struct ChildBodyPartAddedEvent(Entity<SLBodyPartComponent> Part);

public record struct ChildBodyPartRemovedEvent(Entity<SLBodyPartComponent> Part);
