// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.Medical.Cybernetics.Systems;

public abstract class SharedCyberneticsSystem : EntitySystem
{
    [Dependency] protected readonly IPrototypeManager ProtoMan = default!;

    //TODO: this is a stopgap until humanoidCharacter refactor cleans up cybernetics code
    public virtual void ApplyCyberneticVisuals(Entity<HumanoidAppearanceComponent> humanoidAppearance,
        HumanoidCharacterProfile? humanoid)
    {
    }
}