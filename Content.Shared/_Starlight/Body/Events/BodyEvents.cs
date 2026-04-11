// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Body.Components;
namespace Content.Shared._Starlight.Body.Events;



public delegate void BodyPartEventHandler<in TEvent>(Entity<SLBodyComponent> body, Entity<SLBodyPartComponent> bodyPart, TEvent args) where TEvent: notnull;
public delegate void BodyPartRefEventHandler<TEvent>(Entity<SLBodyComponent> body, Entity<SLBodyPartComponent> bodyPart, ref TEvent args) where TEvent: notnull;

public record struct PartAddedToBodyEvent(Entity<SLBodyPartComponent> Part);

public record struct PartRemovedFromBodyEvent(Entity<SLBodyPartComponent> Part);

public record struct ChildBodyPartAddedEvent(Entity<SLBodyPartComponent> Part);

public record struct ChildBodyPartRemovedEvent(Entity<SLBodyPartComponent> Part);
