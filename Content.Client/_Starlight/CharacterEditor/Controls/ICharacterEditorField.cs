// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public interface ICharacterEditorField
{
    public bool Disabled { get; set; }
    public void Randomize();
}