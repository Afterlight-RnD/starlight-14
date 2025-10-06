// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.UI;
using Content.Client._Starlight.UI.Controls;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public sealed class CharacterPreviewModeButton : SLButton, IDropdownControlOption
{
    public CharacterPreviewMode PreviewMode { get; set; } = default;
}