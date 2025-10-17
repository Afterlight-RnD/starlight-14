// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterProfiles;

public record struct CharacterProfileCreatedUIEvent(CharacterProfile Profile, Entity<SpriteComponent> PreviewEntity);

public record struct CharacterProfileUpdatedUIEvent(CharacterProfile Profile);

public record struct CharacterProfileDeletedUIEvent(CharacterProfile Profile);
