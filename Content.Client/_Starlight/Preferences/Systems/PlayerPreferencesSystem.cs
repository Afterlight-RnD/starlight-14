// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Preferences.Components;
using Content.Shared._Starlight.Preferences.Systems;
using Robust.Client.Player;
using Robust.Shared.Network;

namespace Content.Client._Starlight.Preferences.Systems;

public sealed class PlayerPreferencesSystem : SharedPlayerPreferenceSystem
{
    private readonly IPlayerManager _playerManager = default!;
    public override Entity<PlayerPreferencesComponent>? GetPlayerPreferences(NetUserId userId)
    {
        if (userId != _playerManager.LocalUser)
            throw new ArgumentException("Client can only read local user preferences!");
        return GetPlayerPreferences();
    }

    public override Entity<PlayerPreferencesComponent>? GetPlayerPreferences()
    {
        if (EntityQueryEnumerator<PlayerPreferencesComponent>().MoveNext(out var entId, out var preferences))
            return (entId, preferences);
        Log.Error("Player preferences not loaded yet!");
        return null;
    }
}