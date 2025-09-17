// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.CharacterEditor.Systems;

public record struct CharacterProfileSelectedUIEvent(int Slot);

public record struct CharacterProfileEnabledUIEvent(int Slot);

public record struct CharacterProfileDisabledUIEvent(int Slot);