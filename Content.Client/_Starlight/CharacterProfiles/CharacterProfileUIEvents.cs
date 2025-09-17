// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;

namespace Content.Client._Starlight.CharacterProfiles;

public record struct CharacterProfileUpdatedUIEvent(Entity<CharacterProfileComponent> CharacterProfile)
{
    public int Slot => CharacterProfile.Comp.Slot;
};