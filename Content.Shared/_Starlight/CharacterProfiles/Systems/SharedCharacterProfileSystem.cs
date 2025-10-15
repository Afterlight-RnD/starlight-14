// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles.Data;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;

namespace Content.Shared._Starlight.CharacterProfiles.Systems;
public abstract class SharedCharacterProfileSystem : EntitySystem
{
    [Dependency] protected readonly IReflectionManager ReflectionManager = default!;
    [Dependency] protected readonly IDynamicTypeFactory TypeFactory = default!;

    protected List<Type> CharacterProfileDataTypes = new();

    public override void Initialize()
    {
        InitializeCharacterDataTypes();
    }

    public void ApplyToEntity(EntityUid target, CharacterProfile profile, bool isDoll = false)
    {
        foreach (var data in profile.IterateCharacterData())
            RaiseLocalEvent(new ApplyCharacterProfileEvent(target, data, isDoll));
    }

    private void InitializeCharacterDataTypes()
    {
        CharacterProfileDataTypes.Clear();
        var interfaceType = typeof(ICharacterData);
        foreach (var type in ReflectionManager.FindAllTypes())
        {
            if (!type.IsAssignableTo(interfaceType) || type.IsAbstract)
                continue;
            CharacterProfileDataTypes.Add(type);
        }
    }

    protected CharacterProfile CreateRandomProfile()
    {
        var profile = CreateProfile();
        RandomizeProfile(profile);
        return profile;
    }

    public void RandomizeProfile(CharacterProfile profile)
    {
        //TODO: randomization stuff
        ConvertLegacyProfile(profile, HumanoidCharacterProfile.Random());
        profile.MarkDirty();
    }

    protected CharacterProfile CreateProfile()
    {
        var characterData = new List<ICharacterData>();
        foreach (var type in CharacterProfileDataTypes)
            characterData.Add(TypeFactory.CreateInstance<ICharacterData>(type));
        return new CharacterProfile(characterData);
    }

    public void ConvertLegacyProfile(CharacterProfile profile, HumanoidCharacterProfile legacyProfile)
    {
        profile.GetData<LegacyCharacterData>().LegacyProfile = legacyProfile;
        var rolePrefs = profile.GetData<CharacterRolePreferences>();

        rolePrefs.EnabledAntags = new HashSet<ProtoId<AntagPrototype>>(legacyProfile.AntagPreferences);

        rolePrefs.EnabledJobs = new HashSet<ProtoId<JobPrototype>>(legacyProfile.JobPreferences);
        foreach (var (protoId, loadout) in legacyProfile.Loadouts)
        {
            rolePrefs.JobLoadouts.Add(protoId, loadout);
        }

    }

    protected void ClearDirty(CharacterProfile profile)
    {
        profile.ClearDirty();
    }
}