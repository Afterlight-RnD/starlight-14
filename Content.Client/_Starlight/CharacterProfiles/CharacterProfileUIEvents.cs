// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterProfiles;

//== UI Requests ==



//== UI Events ==
public record struct CharacterSlotUpdatedUIEvent(int Slot, CharacterProfile? Profile, Entity<SpriteComponent>? Preview)
{
    public bool Deleted => Profile == null;
}
