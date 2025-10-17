// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles.Data;

[Serializable, NetSerializable]
public partial struct CharacterIdentityData() : ICharacterData
{
    [DataField] public string Name = "";
}

public sealed partial class CharacterIdentityDataSystem : CharacterDataSystem<CharacterIdentityData>,
    ICharacterDataMigration<LegacyCharacterData, CharacterIdentityData>
{

    public override Type[]? ApplyAfterData => [typeof(LegacyCharacterData)];

    protected override void Apply(EntityUid target, CharacterIdentityData data)
    {
        //TODO
    }

    public void MigrateData(LegacyCharacterData oldData, ref CharacterIdentityData newData)
    {
        newData.Name = oldData.LegacyProfile.Name;
    }
}