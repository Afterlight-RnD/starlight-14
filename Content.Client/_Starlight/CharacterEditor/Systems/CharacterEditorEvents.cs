// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public readonly record struct LiveCharacterProfileUpdatedUIEvent(
    Entity<CharacterProfileComponent, HumanoidAppearanceComponent, SpriteComponent> Profile)
{
    public int Slot => Profile.Comp1.Slot;
}

public record struct LiveCharacterProfileDirtiedUIEvent(Entity<CharacterProfileComponent, HumanoidAppearanceComponent, SpriteComponent> Profile)
{
    public int Slot => Profile.Comp1.Slot;
}