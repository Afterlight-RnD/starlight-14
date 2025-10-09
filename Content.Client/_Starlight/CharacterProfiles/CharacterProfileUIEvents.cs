// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Roles;

namespace Content.Client._Starlight.CharacterProfiles;

public record struct CharacterProfileUpdatedUIEvent(
    Entity<CharacterProfileComponent> CharacterProfile,
    JobPrototype PreviewJob)
{
    public int Slot => CharacterProfile.Comp.Slot;
};