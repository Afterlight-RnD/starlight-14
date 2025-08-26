// SPDX-FileCopyrightText: 2025 Afterlight RnD
// SPDX-License-Identifier: ASL-1.2

using System.Linq;
using Content.Client.Lobby;
using Content.Shared.Preferences;
using Robust.Shared.Utility;

namespace Content.Client._Starlight.Character.ProfileSlots;

/// <summary>
/// TODO: fill this out
/// </summary>
public sealed class SLCharacterProfileSystem : EntitySystem
{
    [Dependency] private readonly IClientPreferencesManager _preferences = default!;

    public Action<int, HumanoidCharacterProfile>? OnSlotUpdated = null;

    public override void Initialize()
    {
        if (_preferences.ServerDataLoaded)
        {
            Log.Warning("This should never fire!");
        }
        _preferences.OnServerDataLoaded += OnServerPrefsLoaded;
    }

    public void CreateNewCharacter(HumanoidCharacterProfile profile)
    {
        if (_preferences.Settings == null || _preferences.Preferences == null)
            return;
        var nextSlot = Enumerable.Range(0, _preferences.Settings.MaxCharacterSlots)
            .Except(_preferences.Preferences.Characters.Keys)
            .FirstOrNull();
        if (nextSlot == null)
        {
            Log.Error("Out of character slots!");
            return;
        }
        _preferences.CreateCharacter(profile);
        OnSlotUpdated?.Invoke(nextSlot.Value, profile);
    }


    private void OnServerPrefsLoaded()
    {
        foreach (var (slot, profile) in _preferences.Preferences!.Characters)
        {
            if (profile is HumanoidCharacterProfile characterProfile)
            {
                OnSlotUpdated?.Invoke(slot, characterProfile);
            }
        }

    }
}