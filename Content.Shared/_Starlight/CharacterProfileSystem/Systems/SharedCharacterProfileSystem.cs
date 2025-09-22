// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;

namespace Content.Shared._Starlight.CharacterProfileSystem.Systems;

/// <summary>
/// This handles...
/// </summary>
public abstract class SharedCharacterProfileSystem : EntitySystem
{
    [Dependency] protected readonly IConfigurationManager _cfg = default!;

    protected EntityQuery<CharacterProfileComponent> ProfileQuery;

    protected int MaxProfileSlots = -1;

    public override void Initialize()
    {
        _cfg.OnValueChanged(CCVars.GameMaxCharacterSlots, OnMaxProfileSlotsChanged, true);
    }

    protected virtual void OnMaxProfileSlotsChanged(int newMax)
    {
        MaxProfileSlots = newMax;
    }
}