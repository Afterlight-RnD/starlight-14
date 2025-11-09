// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles.Systems;
using Content.Shared._Starlight.Medical.Cybernetics.Systems;
using Content.Shared.Clothing;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles.Data;

[Serializable, NetSerializable]
public partial struct LegacyCharacterData() : ICharacterData
{
    [DataField] public HumanoidCharacterProfile LegacyProfile = HumanoidCharacterProfile.DefaultWithSpecies();
}

public sealed class LegacyCharacterDataSystem : CharacterDataSystem<LegacyCharacterData>
{
    [Dependency] private readonly SharedHumanoidAppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly LoadoutSystem _loadoutSystem = default!;
    [Dependency] private readonly SharedCyberneticsSystem _cyberneticsSystem = default!;


    public override void Initialize()
    {
        SubscribeProfileEvent<CharacterSpeciesData.SpeciesChangedEvent>(OnSpeciesChanged);
    }

    private void OnSpeciesChanged(CharacterProfile profile, CharacterSpeciesData.SpeciesChangedEvent args)
    {
        var legacyData = profile.GetData<LegacyCharacterData>();
        legacyData.LegacyProfile = legacyData.LegacyProfile.WithSpecies(args.NewSpecies.ID);
        profile.SetData(legacyData);
    }


    protected override void Apply(EntityUid target, LegacyCharacterData data)
    {
        _appearanceSystem.LoadProfile(target, data.LegacyProfile);
        _cyberneticsSystem.ApplyCyberneticVisuals((target, Comp<HumanoidAppearanceComponent>(target)), data.LegacyProfile);
    }

    protected override void Randomize(CharacterProfile profile, ref LegacyCharacterData data)
    {
        data.LegacyProfile = HumanoidCharacterProfile.Random();
    }
}