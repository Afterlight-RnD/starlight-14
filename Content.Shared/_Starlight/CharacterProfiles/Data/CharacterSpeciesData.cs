// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Shared._Starlight.CharacterProfiles.Systems;
using Content.Shared.Humanoid.Prototypes;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles.Data;

[Serializable, NetSerializable]
public partial struct CharacterSpeciesData(): ICharacterData
{
    [DataField] public ProtoId<SpeciesPrototype> BaseSpecies = new();
}

public sealed class CharacterSpeciesDataSystem : CharacterDataSystem<CharacterSpeciesData>
{
    protected override void Apply(EntityUid target, CharacterSpeciesData data)
    {
        //nothing for now
    }

    protected override void Randomize(ref CharacterSpeciesData data)
    {
        data.BaseSpecies = Random.Pick(PrototypeManager
            .EnumeratePrototypes<SpeciesPrototype>()
            .Where(x => x.RoundStart)
            .ToArray()
        ).ID;
    }
}