// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Shared._Starlight.CharacterProfiles.Systems;


public interface ICharacterDataMigrationSystem
{
    public void MigrateProfileData(CharacterProfile profile);
}

public interface ICharacterDataMigration<in TOld, TNew>: ICharacterDataMigrationSystem
    where TOld: struct, ICharacterData
    where TNew: struct, ICharacterData
{
    void ICharacterDataMigrationSystem.MigrateProfileData(CharacterProfile profile)
    {
        var oldData = profile.GetData<TOld>();
        var newData = profile.GetData<TNew>();
        MigrateData(oldData, ref newData);
    }
    public void MigrateData(TOld oldData, ref TNew newData);
}

public interface ICharacterDataSystem
{
    /// <summary>
    /// Apply before other CharacterData
    /// </summary>
    public virtual Type[]? ApplyBeforeData => null;
    /// <summary>
    /// Apply after other CharacterData
    /// </summary>
    public virtual Type[]? ApplyAfterData => null;

    public abstract Type DataType { get; }

    public void SetProfileDefaults(CharacterProfile profile);

    public void ApplyProfile(EntityUid target, CharacterProfile profile);

    public void ApplyProfileToDoll(EntityUid target, CharacterProfile profile, CharacterPreviewMode previewMode);

    public void RandomizeProfile(CharacterProfile profile);

    public bool ValidateProfile(CharacterProfile profile);

    public void FixProfile(CharacterProfile profile);
}

public abstract class CharacterDataSystem<TData> : EntitySystem, ICharacterDataSystem where TData: struct, ICharacterData
{
    [Dependency] protected IPrototypeManager PrototypeManager = default!;
    [Dependency] protected IRobustRandom Random = default!;

    /// <summary>
    /// Apply before other CharacterData
    /// </summary>
    public virtual Type[]? ApplyBeforeData => null;
    /// <summary>
    /// Apply after other CharacterData
    /// </summary>
    public virtual Type[]? ApplyAfterData => null;


    Type ICharacterDataSystem.DataType => typeof(TData);

    protected abstract void Apply(EntityUid target, TData data);

    protected virtual void ApplyToDoll(EntityUid target, TData data, CharacterPreviewMode previewMode)
    {
        Apply(target, data);
    }

    protected virtual void Randomize(ref TData data){}

    protected virtual bool Validate(TData data) => true;

    /// <summary>
    /// Fixes any malformed/malicious data in the profile
    /// </summary>
    /// <param name="data">data to change</param>
    /// <returns>True if changes were made</returns>
    protected virtual bool FixData(ref TData data) => false;

    /// <summary>
    /// Applies default values to a profile
    /// </summary>
    /// <param name="data">data to change</param>
    /// <returns>True if changes were made</returns>
    protected virtual void SetDefaults(ref TData data) {}

    void ICharacterDataSystem.ApplyProfile(EntityUid target, CharacterProfile profile)
    {
        Apply(target, profile.GetData<TData>());
    }

    void ICharacterDataSystem.ApplyProfileToDoll(EntityUid target, CharacterProfile profile, CharacterPreviewMode previewMode)
    {
        ApplyToDoll(target, profile.GetData<TData>(),previewMode);
    }

    void ICharacterDataSystem.RandomizeProfile(CharacterProfile profile)
    {
        var data = new TData();
        profile.SetData(data);
    }

    bool ICharacterDataSystem.ValidateProfile(CharacterProfile profile)
    {
        return Validate(profile.GetData<TData>());
    }

    void ICharacterDataSystem.FixProfile(CharacterProfile profile)
    {
        var data = profile.GetData<TData>();
        if (!FixData(ref data))
            return;
        profile.SetData(data);
    }

    void ICharacterDataSystem.SetProfileDefaults(CharacterProfile profile)
    {
        var data = new TData();
        SetDefaults(ref data);

        profile.SetData(data, false);
    }
}
