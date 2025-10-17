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

    public bool AddProfile(int slot, CharacterProfile profile)
    {
        profile.Slot = slot;
        return AddProfile(profile);
    }

    public bool AddProfile(CharacterProfile profile)
    {
        return _profiles.TryAdd(profile.Slot, profile);
    }

    public bool SlotHasProfile(int slot)
    {
        return _profiles.ContainsKey(slot);
    }

    public int OccupiedSlots => _profiles.Count;

    public int GetFirstFreeSlot(int maxCharacters)
    {
        for (var i = 0; i < maxCharacters; i++)
            if (!_profiles.ContainsKey(i))
                return i;
        return -1;
    }

    public bool TryGetCharacterProfile(int slot, [NotNullWhen(true)] out CharacterProfile? profile)
    {
        return _profiles.TryGetValue(slot, out profile);
    }
    public void SetProfileData(int slot, List<ICharacterData> characterData)
    {
        _profiles[slot].SetData(characterData);
    }

    private void SetProfileData<T>(int slot, List<ICharacterData> characterData)
    {
        _profiles[slot].SetData(characterData);
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

    public bool DeleteProfile(int slot,  [NotNullWhen(true)] out CharacterProfile? profile)
    {
        return _profiles.Remove(slot, out profile);
    }
}