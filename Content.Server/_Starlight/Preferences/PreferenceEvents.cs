// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared.Preferences;
using Robust.Shared.Player;

namespace Content.Server.Preferences.Managers;


[ByRefEvent]
public record struct PlayerPreferencesLoadedEvent(ICommonSession Session, PlayerPreferences? Preferences);

[ByRefEvent]
public record struct PlayerPreferencesUnloadedEvent(ICommonSession Session);