// SPDX-FileCopyrightText: 2025 Afterlight RnD
// SPDX-License-Identifier: ASL-1.2

using Content.Shared.Preferences;

namespace Content.Client._Starlight.Character.ProfileSlots;

public record struct CharacterSlotUpdatedUIEvent(int SlotIdx, HumanoidCharacterProfile? Profile);