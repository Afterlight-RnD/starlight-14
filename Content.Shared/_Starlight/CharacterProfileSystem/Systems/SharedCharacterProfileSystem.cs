// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;

namespace Content.Shared._Starlight.CharacterProfileSystem.Systems;

/// <summary>
/// This handles...
/// </summary>
public abstract class SharedCharacterProfileSystem : EntitySystem
{
    protected EntityQuery<CharacterProfileComponent> ProfileQuery;
}