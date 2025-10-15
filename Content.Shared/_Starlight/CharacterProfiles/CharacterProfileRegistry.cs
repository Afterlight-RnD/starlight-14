// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using System.Diagnostics.CodeAnalysis;

namespace Content.Shared._Starlight.CharacterProfiles;

public sealed class CharacterProfileRegistry
{
    private Dictionary<int, CharacterProfile> _profiles = new();

    public CharacterProfile this[int slot]
    {
        get
        {
            return _profiles[slot];
        }
        set
        {
            _profiles[slot] = value;
        }
    }

    public bool TryGetCharacterProfile(int slot, [NotNullWhen(true)] out CharacterProfile? profile)
    {
        return _profiles.TryGetValue(slot, out profile);
    }

    public void SetProfile(int characterProfile, CharacterProfile profile)
    {
        _profiles[characterProfile] = profile;
    }

    public void MarkDirtyProfileData<T>(int slot) where T: ICharacterData, new()
    {
        if (!TryGetCharacterProfile(slot, out var profile))
            return;
        profile.MarkDataDirty<T>();
    }

    public void MarkDirtyProfile(int slot)
    {
        if (!TryGetCharacterProfile(slot, out var profile))
            return;
        profile.MarkDirty();
    }

    public void ClearDirty(int slot)
    {
        if (!TryGetCharacterProfile(slot, out var profile))
            return;
        profile.ClearDirty();
    }

    public bool DeleteProfile(int slot)
    {
        return _profiles.Remove(slot);
    }
}