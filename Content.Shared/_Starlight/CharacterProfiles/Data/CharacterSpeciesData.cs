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
    private const int AbsoluteMinAge = 18;

    [DataField] public ProtoId<SpeciesPrototype> BaseSpecies = new();
    [DataField] public string CustomSpeciesName = string.Empty;
    [DataField] public EntProtoId DollPrototype = new();

    [DataField] public int MinAge = AbsoluteMinAge;
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
        speciesData.DollPrototype = proto.DollPrototype;

        speciesData.MinAge = proto.MinAge;
        speciesData.MaxAge = proto.MaxAge;

        speciesData.OldAge = proto.OldAge;
        speciesData.YoungAge = proto.YoungAge;
        profile.SetData(speciesData);
        var identityData = profile.GetData<CharacterIdentityData>();
        identityData.PhysicalAge = int.Clamp(identityData.PhysicalAge, speciesData.MinAge, speciesData.MaxAge);
        profile.SetData(identityData);
    }


    public void ChangeSpecies(CharacterProfile profile, ProtoId<SpeciesPrototype> newSpecies)
    {
        var existing = profile.GetData<CharacterSpeciesData>();
        if (existing.BaseSpecies == newSpecies)
            return;
        existing.BaseSpecies = newSpecies;
        profile.SetData(existing);
        RaiseProfileEvent(profile, new CharacterSpeciesData.SpeciesChangedEvent(PrototypeManager.Index(newSpecies)));
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