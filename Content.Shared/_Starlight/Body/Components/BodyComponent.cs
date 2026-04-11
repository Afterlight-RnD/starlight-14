// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Body.Prototypes;
using Content.Shared._Starlight.Body.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Body.Components;

[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(false, true)]
public sealed partial class SLBodyComponent : Component
{
    [DataField, AutoNetworkedField, Access(typeof(BodySystem))]
    public EntityUid? RootBodyPart { get; set; } = null;

    //TODO: horrible kludge, replace with actual caching later!
    public Entity<SLBodyPartComponent> CachedRootPart =>
        (RootBodyPart!.Value, IoCManager.Resolve<IEntityManager>().GetComponent<SLBodyPartComponent>(RootBodyPart!.Value));

    [DataField, AutoNetworkedField, Access(typeof(BodySystem))]
    public List<EntityUid> BodyParts = new();

    [DataField, AutoNetworkedField, Access(typeof(BodySystem))]
    public Dictionary<ProtoId<BodyPartTypePrototype>, List<EntityUid>> TypedBodyParts = new();
}
