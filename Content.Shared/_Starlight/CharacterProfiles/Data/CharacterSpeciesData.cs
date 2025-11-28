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
public sealed partial class CharacterSpeciesData(): CharacterData
{
    [DataField] public ProtoId<SpeciesPrototype> BaseSpecies = new();
    [DataField] public string CustomSpeciesName = string.Empty;
    [DataField] public EntProtoId DollPrototype = new();

    [DataField] public int MinAge = 18;
    [DataField] public int MaxAge = int.MaxValue;
    [DataField] public int YoungAge = 25;
    [DataField] public int OldAge = 65;

    public record struct SpeciesChangedEvent(SpeciesPrototype NewSpecies);
}

public sealed class CharacterSpeciesDataSystem : CharacterDataSystem<CharacterSpeciesData>,
    ICharacterDataMigration<LegacyCharacterData, CharacterSpeciesData>
{
    public override Type[]? ApplyAfterData => [typeof(LegacyCharacterData)];

    public override bool HasInit => true;

    public override void Initialize()
    {
        RegisterProtoReloadListener<SpeciesPrototype>(OnSpeciesProtoReloaded);
    }

    private void OnSpeciesProtoReloaded(CharacterProfile profile)
    {

        var speciesData = profile.GetData<CharacterSpeciesData>();
        var proto = PrototypeManager.Index(speciesData.BaseSpecies);
        ChangeSpecies(profile, proto);
    }

    public void ChangeSpecies(CharacterProfile profile, SpeciesPrototype newSpecies)
    {
        var existing = profile.GetData<CharacterSpeciesData>();
        if (existing.BaseSpecies == newSpecies)
            return;
        existing.BaseSpecies = newSpecies;
        existing.DollPrototype = newSpecies.DollPrototype;

        existing.MaxAge = newSpecies.MaxAge;
        existing.MinAge = newSpecies.MinAge;

        existing.OldAge = newSpecies.OldAge;
        existing.YoungAge = newSpecies.YoungAge;
        RaiseProfileEvent(profile, new CharacterSpeciesData.SpeciesChangedEvent(newSpecies));
    }

    public void ChangeSpecies(CharacterProfile profile, ProtoId<SpeciesPrototype> newSpecies)
    {
       ChangeSpecies(profile,PrototypeManager.Index(newSpecies));
    }

    protected override void InitData(CharacterProfile profile,  ref CharacterSpeciesData data)
    {
        data.DollPrototype = PrototypeManager.Index(data.BaseSpecies).DollPrototype;
    }

    protected override void Apply(EntityUid target, CharacterSpeciesData data)
    {
        //nothing for now
    }

    protected override void Randomize(CharacterProfile profile, ref CharacterSpeciesData data)
    {
        data.BaseSpecies = Random.Pick(PrototypeManager
            .EnumeratePrototypes<SpeciesPrototype>()
            .Where(x => x.RoundStart)
            .ToArray()
        ).ID;
        data.DollPrototype = PrototypeManager.Index(data.BaseSpecies).DollPrototype;
    }

    public void MigrateData(LegacyCharacterData oldData, ref CharacterSpeciesData newData)
    {
        newData.BaseSpecies = oldData.LegacyProfile.Species;
        newData.CustomSpeciesName = oldData.LegacyProfile.CustomSpecieName;
        var speciesProto = PrototypeManager.Index(newData.BaseSpecies);

        newData.MinAge = speciesProto.MinAge;
        newData.MaxAge = speciesProto.MaxAge;
        newData.YoungAge = speciesProto.YoungAge;
        newData.OldAge = speciesProto.OldAge;
    }
}