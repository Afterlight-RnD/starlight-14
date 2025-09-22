// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterEditor.Systems;

//UIEvents
public record struct CharacterProfileSelectedUIEvent(int Slot);

//UIEvents
public record struct ActiveCharacterProfileUpdatedUIEvent(
    int Slot,
    Entity<CharacterProfileComponent> Profile,
    Entity<SpriteComponent, HumanoidAppearanceComponent> Preview);

public record struct CharacterProfileEnabledUIEvent(int Slot);

public record struct CharacterProfileDisabledUIEvent(int Slot);