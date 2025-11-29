// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterEditor.Widgets;

public sealed class CharacterSlotSelectorWidgetSystem: BoundUISystem<CharacterSlotSelectorWidget>
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    protected override void BoundControlEnteredTree(CharacterSlotSelectorWidget boundControl)
    {
    }

    protected override void BoundControlExitedTree(CharacterSlotSelectorWidget boundControl)
    {

    }
}