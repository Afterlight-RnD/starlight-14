// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Preferences.Components;
using Content.Shared.Preferences;
using Robust.Shared.Player;

namespace Content.Server.Preferences.Managers;


public record struct PlayerPreferencesLoadedEvent(ICommonSession Session, Entity<PlayerPreferencesComponent> Preferences);

public record struct LegacyPlayerPreferencesLoadStartedEvent(ICommonSession Session);

public record struct LegacyPlayerPreferencesLoadedEvent(ICommonSession Session, PlayerPreferences? Preferences);

public record struct LegacyPlayerPreferencesUnloadedEvent(ICommonSession Session);