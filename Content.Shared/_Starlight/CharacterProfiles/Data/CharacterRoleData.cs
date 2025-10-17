// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Shared._Starlight.CharacterProfiles.Systems;
using Content.Shared.Clothing;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles.Data;

[Serializable, NetSerializable]
public partial struct CharacterRoleData() : ICharacterData
{
    [DataField] public ProtoId<JobPrototype> FavoriteJob = new();

    [DataField] public Dictionary<ProtoId<AntagPrototype>, RoleLoadout> AntagLoadouts = new();
    [DataField] public HashSet<ProtoId<AntagPrototype>> EnabledAntags = new();

    [DataField] public Dictionary<ProtoId<JobPrototype>, RoleLoadout> JobLoadouts = new();
    [DataField] public HashSet<ProtoId<JobPrototype>> EnabledJobs = new();
}


public sealed partial class CharacterRoleDataSystem : CharacterDataSystem<CharacterRoleData>,
    ICharacterDataMigration<LegacyCharacterData, CharacterRoleData>
{
    [Dependency] private readonly LoadoutSystem _loadoutSystem = default!;

    public override Type[]? ApplyAfterData => [typeof(LegacyCharacterData)];

    protected override void ApplyToDoll(EntityUid target, CharacterRoleData data, CharacterPreviewMode previewMode)
    {
        if (previewMode == CharacterPreviewMode.Nude)
            return;
        _loadoutSystem.ApplyJobClothes(target, data);
    }

    protected override void Apply(EntityUid target, CharacterRoleData data)
    {
        //empty for now.
    }

    public void MigrateData(LegacyCharacterData oldData, ref CharacterRoleData newData)
    {
        newData.EnabledAntags = new(oldData.LegacyProfile.AntagPreferences);
        newData.EnabledJobs = new(oldData.LegacyProfile.JobPreferences);

        foreach (var (protoId, roleLoadout) in oldData.LegacyProfile.Loadouts)
            newData.JobLoadouts.Add(protoId, roleLoadout);
        newData.FavoriteJob = newData.EnabledJobs.First(); //TODO: temp because we don't save favorite jobs
    }
}