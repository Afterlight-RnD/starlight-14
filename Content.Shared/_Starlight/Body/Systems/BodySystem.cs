// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Body.Components;
using Content.Shared._Starlight.Body.Events;
using Robust.Shared.Utility;

namespace Content.Shared._Starlight.Body.Systems;

public sealed partial class BodySystem : EntitySystem
{
    public override void Initialize()
    {
    }

    public bool AddBodyPart(Entity<SLBodyComponent> target, Entity<SLBodyPartComponent> bodyPart)
    {
        if (!bodyPart.Comp.IsOrphan)
            return false;
        if (target.Comp.RootBodyPart.HasValue)
            return AddBodyPart(
                (target.Comp.RootBodyPart.Value, Comp<SLBodyPartComponent>(target.Comp.RootBodyPart.Value)), bodyPart);
        target.Comp.RootBodyPart = bodyPart;
        RegisterPartWithBody(target,bodyPart);
        return true;
    }

    public bool AddBodyPart(Entity<SLBodyPartComponent> parentPart, Entity<SLBodyPartComponent> bodyPart)
    {
        if (!bodyPart.Comp.IsOrphan)
            return false;
        AddChildBodyPart(parentPart, bodyPart);
        if (parentPart.Comp.OwningBody.HasValue)
            RegisterPartWithBody((parentPart.Comp.OwningBody.Value, Comp<SLBodyComponent>(parentPart.Comp.OwningBody.Value)), bodyPart);
        return true;
    }
    public bool OrphanBodyPart(Entity<SLBodyPartComponent> bodyPart)
    {
        if (!bodyPart.Comp.ParentPart.HasValue)
            return false;
        RemoveChildPart((bodyPart.Comp.ParentPart.Value, Comp<SLBodyPartComponent>(bodyPart.Comp.ParentPart.Value)),
            bodyPart);
        if (bodyPart.Comp.OwningBody.HasValue)
            DeRegisterFromBody((bodyPart.Comp.OwningBody.Value, Comp<SLBodyComponent>(bodyPart.Comp.OwningBody.Value)), bodyPart);
        return true;
    }

    #region InternalPartAPI
    private void AddChildBodyPart(
        Entity<SLBodyPartComponent> parentPart,
        Entity<SLBodyPartComponent> bodyPart)
    {
        parentPart.Comp.ChildParts.Add(bodyPart);
        bodyPart.Comp.ParentPart = parentPart;
        bodyPart.Comp.OwningBody = parentPart.Comp.OwningBody;
        RaiseLocalEvent(parentPart, new ChildBodyPartAddedEvent(bodyPart));
        Dirty(parentPart);
        Dirty(bodyPart);
    }

    private bool RemoveChildPart(Entity<SLBodyPartComponent> parentPart, Entity<SLBodyPartComponent> childPart)
    {
        if (!parentPart.Comp.ChildParts.Remove(childPart))
            return false;
        RaiseLocalEvent(parentPart, new ChildBodyPartRemovedEvent(parentPart));
        childPart.Comp.ParentPart = null;
        Dirty(parentPart);
        Dirty(childPart);
        return true;
    }

    private void RegisterPartWithBody(Entity<SLBodyComponent> body, Entity<SLBodyPartComponent> bodyPart)
    {
        body.Comp.BodyParts.Add(bodyPart);
        body.Comp.TypedBodyParts.GetOrNew(bodyPart.Comp.PartType).Add(bodyPart);
        RaiseLocalEvent(body.Owner, new PartAddedToBodyEvent(bodyPart));
        Dirty(body);
    }


    private bool DeRegisterFromBody(Entity<SLBodyComponent> body, Entity<SLBodyPartComponent> bodyPart)
    {
        if (bodyPart.Comp.IsOrphan)
            return false;
        RaiseLocalEvent(body.Owner, new PartRemovedFromBodyEvent(bodyPart));
        if (!body.Comp.BodyParts.Contains(bodyPart))
            return false;
        if ( body.Comp.TypedBodyParts.TryGetValue(bodyPart.Comp.PartType, out var existing))
            existing.Remove(bodyPart);
        bodyPart.Comp.OwningBody = null;
        Dirty(body);
        Dirty(bodyPart);
        return true;
    }
    #endregion
}
