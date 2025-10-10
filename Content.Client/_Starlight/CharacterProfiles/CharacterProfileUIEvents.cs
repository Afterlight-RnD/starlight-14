// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Humanoid;
using Content.Shared.Roles;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterProfiles;

public readonly record struct CharacterProfileUpdatedUIEvent(
    Entity<CharacterProfileComponent> CharacterProfile)
{
    public int Slot => CharacterProfile.Comp.Slot;
};

public record struct CharacterProfileAddedUIEvent(
    Entity<CharacterProfileComponent> Profile,
    Entity<SpriteComponent, HumanoidAppearanceComponent> ProfileDoll)
{
    public int Slot => Profile.Comp.Slot;
};

public record struct CharacterProfileRemovedUIEvent(
    Entity<CharacterProfileComponent> Profile)
{
    public int Slot => Profile.Comp.Slot;
};

public record struct CharacterProfileEnabledUIEvent(Entity<CharacterProfileComponent> Profile);

public record struct CharacterProfileDisabledUIEvent(Entity<CharacterProfileComponent> Profile);
