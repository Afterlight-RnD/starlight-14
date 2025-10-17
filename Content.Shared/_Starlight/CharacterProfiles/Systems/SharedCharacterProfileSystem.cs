// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Shared._Starlight.CharacterProfiles.Data;
using Content.Shared.CCVar;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;

namespace Content.Shared._Starlight.CharacterProfiles.Systems;
public abstract class SharedCharacterProfileSystem : EntitySystem
{
    [Dependency] protected readonly IReflectionManager ReflectionManager = default!;
    [Dependency] protected readonly IConfigurationManager Cfg = default!;
    [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;

    protected int MaxCharacters = -1;
    protected readonly List<ICharacterDataSystem> CharacterDataSystems = new();
    protected readonly List<ICharacterDataMigrationSystem> CharacterDataMigrations = new();
    protected ICharacterDollProvider DollProvider = default!;//No provider or multiple providers will cause a fatal error

    public override void Initialize()
    {
        CacheReflectionData();
        Cfg.OnValueChanged(CCVars.GameMaxCharacterSlots, MaxCharactersChanged);
    }

    private void MaxCharactersChanged(int slots)
    {
        MaxCharacters = slots;
    }

    public bool ValidateProfile(CharacterProfile profile)
    {
        foreach (var dataSystem in CharacterDataSystems)
            if (!dataSystem.ValidateProfile(profile))
            {
                profile.HasInvalidData = true;
                return false;
            }
        return true;
    }

    public void FixProfile(CharacterProfile profile)
    {
        //No invalid data means nothing to fix!
        if (!profile.HasInvalidData)
            return;
        foreach (var dataSystem in CharacterDataSystems)
            dataSystem.FixProfile(profile);
    }

    public void ApplyToEntity(EntityUid target, CharacterProfile profile)
    {
        foreach (var dataSystem in CharacterDataSystems)
            dataSystem.ApplyProfile(target, profile);
    }

    public void ApplyToDoll(EntityUid target, CharacterProfile profile, CharacterPreviewMode previewMode = default)
    {
        foreach (var dataSystem in CharacterDataSystems)
            dataSystem.ApplyProfileToDoll(target, profile, previewMode);
    }

    private void CacheReflectionData()
    {
        CharacterDataSystems.Clear();
        var dataSystemInterfaceType = typeof(ICharacterDataSystem);

        var foundDollProvider = false;
        //Get and cache all datasystems
        foreach (var type in ReflectionManager.FindAllTypes())
        {
            if (type.IsAssignableTo(dataSystemInterfaceType) && !type.IsAbstract)
            {
                var dataSystem = (ICharacterDataSystem)EntityManager.EntitySysManager.GetEntitySystem(type);
                CharacterDataSystems.Add(dataSystem);
                if (type.IsAssignableTo(typeof(ICharacterDataMigrationSystem)))
                    CharacterDataMigrations.Add((ICharacterDataMigrationSystem)dataSystem);
                if (type.IsAssignableTo(typeof(ICharacterDollProvider)))
                {
                    if (foundDollProvider)
                        Log.Fatal($"Multiple CharacterDollProviders found, only one is supported! {DollProvider.GetType()}, {type}");
                    DollProvider = (ICharacterDollProvider)dataSystem;
                    foundDollProvider = true;
                }
            }
        }
        //sort data systems so that methods get run according to characterData order
        CharacterDataSystems.Sort((s1, s2) =>
        {
            if (s1.ApplyBeforeData != null && s1.ApplyBeforeData.Contains(s2.DataType))
                return -1;
            if (s1.ApplyAfterData != null && s1.ApplyAfterData.Contains(s2.DataType))
                return 1;
            return 0;
        });
    }

    protected CharacterProfile CreateRandomProfile()
    {
        var profile = CreateProfile();
        RandomizeProfile(profile, false);
        return profile;
    }

    public void RandomizeProfile(CharacterProfile profile, bool dirtyProfile = true)
    {
        foreach (var dataSystem in CharacterDataSystems)
            dataSystem.RandomizeProfile(profile);
        profile.DollPrototype = DollProvider.GetDollProto(profile);
        if (dirtyProfile)
            profile.MarkDirty();
    }

    /// <summary>
    /// Create a profile from existing data
    /// </summary>
    /// <param name="existingData">pre-existing data</param>
    /// <returns>new profile</returns>
    protected CharacterProfile LoadExistingProfile(List<ICharacterData> existingData)
    {
        var newProfile = new CharacterProfile(existingData);
        //Migrate existing data
        foreach (var migrationSystem in CharacterDataMigrations)
            migrationSystem.MigrateProfileData(newProfile);

        newProfile.DollPrototype = DollProvider.GetDollProto(newProfile);
        return newProfile;
    }

    /// <summary>
    /// Create a new default profile
    /// </summary>
    /// <returns></returns>
    protected CharacterProfile CreateProfile()
    {
        var newProfile = new CharacterProfile();
        foreach (var dataSystem in CharacterDataSystems)
            dataSystem.SetProfileDefaults(newProfile);
        foreach (var migrationSystem in CharacterDataMigrations)
            migrationSystem.MigrateProfileData(newProfile);

        newProfile.DollPrototype = DollProvider.GetDollProto(newProfile);
        return newProfile;
    }

    protected void ClearDirty(CharacterProfile profile)
    {
        profile.ClearDirty();
    }

    public EntityUid CreateProfileDoll(CharacterProfile profile, CharacterPreviewMode previewMode = default)
    {
        var doll = EntityManager.Spawn(profile.DollPrototype);
        ApplyToDoll(doll, profile, previewMode);
        return doll;
    }
}