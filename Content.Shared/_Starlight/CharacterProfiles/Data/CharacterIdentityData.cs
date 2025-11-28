// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles.Systems;
using Content.Shared.Humanoid;
using Robust.Shared.Enums;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles.Data;

[Serializable, NetSerializable]
public sealed partial class CharacterIdentityData() : CharacterData
{
    [DataField] public string Name = string.Empty;
    [DataField] public string Nickname = string.Empty;
    [DataField] public string LastName = string.Empty;

    [DataField] public Sex BodyType = default;
    [DataField] public Gender Gender = default;

    [DataField] public int PhysicalAge = 18;
    [DataField] public int ChronologicalAge = 18;
}

public sealed partial class CharacterIdentityDataSystem : CharacterDataSystem<CharacterIdentityData>,
    ICharacterDataMigration<LegacyCharacterData, CharacterIdentityData>
{

    public override Type[]? ApplyAfterData => [typeof(LegacyCharacterData)];

    public override void Initialize()
    {
        SubscribeProfileEvent<CharacterSpeciesData.SpeciesChangedEvent>(OnSpeciesChanged);
    }

    private void OnSpeciesChanged(CharacterProfile profile, CharacterSpeciesData.SpeciesChangedEvent args)
    {
        var identityData = profile.GetData<CharacterIdentityData>();
        identityData.PhysicalAge = int.Clamp(identityData.PhysicalAge, args.NewSpecies.MinAge, args.NewSpecies.MaxAge);

    }

    protected override void Apply(EntityUid target, CharacterIdentityData data)
    {
        //TODO
    }

    public void MigrateData(LegacyCharacterData oldData, ref CharacterIdentityData newData)
    {
        newData.Name = oldData.LegacyProfile.Name;
        newData.PhysicalAge = oldData.LegacyProfile.Age;
        newData.ChronologicalAge = oldData.LegacyProfile.Age;
        newData.BodyType = oldData.LegacyProfile.Sex;
        newData.Gender = oldData.LegacyProfile.Gender;
    }
}