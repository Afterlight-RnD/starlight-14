// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Preferences.Components;
using Robust.Shared.Network;

namespace Content.Shared._Starlight.Preferences.Systems;

/// <summary>
/// This handles player preference data
/// </summary>
public abstract class SharedPlayerPreferenceSystem : EntitySystem
{
    public abstract Entity<PlayerPreferencesComponent>? GetPlayerPreferences(NetUserId userId);

    public virtual Entity<PlayerPreferencesComponent>? GetPlayerPreferences()
    {
        throw new NotSupportedException("Server must pass in a userId");
    }
}