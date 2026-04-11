// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Body.Prototypes;

[Prototype]
public sealed partial class BodySlotPrototype : IPrototype
{
    public required string ID { get; set; }

    [DataField] public HashSet<ProtoId<BodyPartTypePrototype>>? AllowedTypes = null;
}
