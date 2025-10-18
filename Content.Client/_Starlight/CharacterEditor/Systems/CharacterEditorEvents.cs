// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public record struct CharacterEditingStartedUIEvent(CharacterProfile Profile, Entity<SpriteComponent> PreviewEntity);

public record struct CharacterEditingUpdateUIEvent(CharacterProfile Profile);

public record struct CharacterEditingUpdatedPreviewEntityUIEvent(Entity<SpriteComponent> PreviewEntity);

public record struct CharacterEditingHasChangesUIEvent(CharacterProfile Profile);

public record struct CharacterEditingFinishedUIEvent(CharacterProfile Profile);

public record struct CharacterEditingAppliedUIEvent(CharacterProfile Profile);

public record struct CharacterProfileToggleActiveUIEvent(int Slot, bool Active);