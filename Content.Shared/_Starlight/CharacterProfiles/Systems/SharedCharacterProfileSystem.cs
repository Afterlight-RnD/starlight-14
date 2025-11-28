// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Shared._Starlight.CharacterProfiles.Data;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;
using Robust.Shared.Reflection;

namespace Content.Shared._Starlight.CharacterProfiles.Systems;
public abstract partial class SharedCharacterProfileSystem : EntitySystem
{
    [Dependency] protected readonly IReflectionManager ReflectionManager = default!;
    [Dependency] protected readonly IConfigurationManager Cfg = default!;
    [Dependency] protected readonly IPrototypeManager PrototypeManager = default!;

    protected int MaxCharacters = -1;
    protected readonly List<ICharacterDataSystem> CharacterDataSystems = new();
    protected readonly List<ICharacterDataMigrationSystem> CharacterDataMigrations = new();
    protected readonly List<ICharacterDataSystem> InitSystems = new();

    protected Dictionary<Type, (Action<SharedCharacterProfileSystem, CharacterProfile, Action<CharacterProfile>>, Action<CharacterProfile>)> ProtoReloadEvents = new();

    public override void Initialize()
    {
        CacheReflectionData();
        Cfg.OnValueChanged(CCVars.GameMaxCharacterSlots, MaxCharactersChanged);
        PrototypeManager.PrototypesReloaded += HandleProtoReloaded;
    }

    public override void Shutdown()
    {
        base.Shutdown();
        PrototypeManager.PrototypesReloaded -= HandleProtoReloaded;
        ProtoReloadEvents.Clear();
    }
    protected virtual void HandleProtoReloaded(PrototypesReloadedEventArgs args) {}

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

        //Get and cache all datasystems
        foreach (var type in ReflectionManager.FindAllTypes())
        {
            if (type.IsAssignableTo(dataSystemInterfaceType) && !type.IsAbstract)
            {
                var dataSystem = (ICharacterDataSystem)EntityManager.EntitySysManager.GetEntitySystem(type);
                CharacterDataSystems.Add(dataSystem);
                if (dataSystem.HasInit)
                    InitSystems.Add(dataSystem);
                if (type.IsAssignableTo(typeof(ICharacterDataMigrationSystem)))
                    CharacterDataMigrations.Add((ICharacterDataMigrationSystem)dataSystem);
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
        if (dirtyProfile)
            profile.MarkDirty();
    }

    /// <summary>
    /// Create a profile from existing data
    /// </summary>
    /// <param name="existingData">pre-existing data</param>
    /// <returns>new profile</returns>
    protected CharacterProfile LoadExistingProfile(List<CharacterData> existingData)
    {
        var newProfile = new CharacterProfile(existingData);
        //Migrate existing data
        foreach (var migrationSystem in CharacterDataMigrations)
            migrationSystem.MigrateProfileData(newProfile);
        foreach (var dataSystem in InitSystems)
            dataSystem.InitProfile(newProfile);
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
        foreach (var dataSystem in InitSystems)
            dataSystem.InitProfile(newProfile);
        return newProfile;
    }

    protected void ClearDirty(CharacterProfile profile)
    {
        profile.ClearDirty();
    }

    public EntityUid CreateProfileDoll(CharacterProfile profile, CharacterPreviewMode previewMode = default)
    {
        var doll = EntityManager.Spawn(profile.GetData<CharacterSpeciesData>().DollPrototype);
        ApplyToDoll(doll, profile, previewMode);
        return doll;
    }

    public void RaiseProfileEvent<TEvent>(CharacterProfile profile, TEvent args) where TEvent : struct
    {
        RaiseLocalEvent(new ProfileEvent<TEvent>(profile, args));
    }

    protected void RaiseProtoReloadOnProfile(HashSet<Type> changeSet, CharacterProfile profile)
    {
        foreach (var type in changeSet)
        {
            if (!ProtoReloadEvents.TryGetValue(type, out var data))
                continue;
            data.Item1.Invoke(this, profile, data.Item2);
        }
    }

    public void RegisterProtoReloadListener<TProto>(Action<CharacterProfile> handler) where TProto : class, IPrototype
    {
        if (!ProtoReloadEvents.TryAdd(typeof(TProto), (static (system, profile, handlerIn) =>
            {
                system.RaiseProfileEvent(profile, new PrototypeReloadedProfileEvent<TProto>(handlerIn));
            }, handler)))
            Log.Error($"Tried to register protoReload event twice for proto:{typeof(TProto)}");
    }

    public record struct ProfileEvent<TEvent>(CharacterProfile Profile, TEvent Event) where TEvent : struct;

    public record struct PrototypeReloadedProfileEvent<TProto>(Action<CharacterProfile> Handler) where TProto: class, IPrototype;
}