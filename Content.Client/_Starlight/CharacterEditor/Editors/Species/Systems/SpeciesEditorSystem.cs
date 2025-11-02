// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Editors.Species.Controls;
using Content.Client._Starlight.UI.Core;
using Content.Shared.Humanoid.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterEditor.Editors.Species.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class SpeciesEditorSystem : UISystem
{
    [Dependency] private readonly IUserInterfaceManager _uiMan = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly SpriteSystem _spriteSystem = default!;
    public override void Initialize()
    {
    }

    // private void OnSpeciesGroupUIAdded(SpeciesGroupList speciesGroupList, ControlAddedUIEvent ev)
    // {
    //     foreach (var species in _prototypeManager.EnumeratePrototypes<SpeciesPrototype>())
    //     {
    //         if (!species.RoundStart)
    //             continue;
    //         speciesGroupList.AddSpecies(species, _spriteSystem.Frame0(species.SpeciesIcon), Loc.GetString(species.Name));
    //     }
    // }
    //
    // private void OnSpeciesGroupUIRemoved(SpeciesGroupList speciesGroupList, ControlOrphanedUIEvent ev)
    // {
    //     speciesGroupList.ClearSpecies();
    // }
}