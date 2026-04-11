// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Body.Prototypes;
using Content.Shared._Starlight.Body.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Body.Components;
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(false)]
public sealed partial class SLBodyPartComponent : Component
{
    public bool HasBody => OwningBody != null;
    public bool IsRootPart => ParentPart == null;

    public bool IsOrphan => !HasBody && IsRootPart;

    [DataField, AutoNetworkedField, Access(typeof(BodySystem), typeof(SLBodyComponent))]
    public EntityUid? OwningBody { get; set; }

    [DataField, AutoNetworkedField, Access(typeof(BodySystem), typeof(SLBodyComponent))]
    public EntityUid? ParentPart { get; set; }

    [DataField(required:true), AutoNetworkedField, Access(typeof(BodySystem), typeof(SLBodyComponent))]
    public ProtoId<BodyPartTypePrototype> PartType { get; set; }

    [DataField, AutoNetworkedField, Access(typeof(BodySystem), typeof(SLBodyComponent))]
    public List<EntityUid> ChildParts = new();
}
