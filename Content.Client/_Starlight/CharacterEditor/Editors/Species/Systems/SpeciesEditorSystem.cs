// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Editors.Species.Controls;
using Content.Shared.Humanoid.Prototypes;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.UIEvents;
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

    private SpeciesSelectorButton? _selectedButton = null;
    public override void Initialize()
    {
        SubscribeUIEvent<GroupedSpeciesList, ControlAddedUIEvent>(OnSpeciesGroupUIAdded);
        SubscribeUIEvent<GroupedSpeciesList, ControlRemovedUIEvent>(OnSpeciesGroupUIRemoved);
        SubscribeUIEvent<SpeciesSelectorButton, SpeciesSelectorButton.SelectedUIEvent>(OnButtonSelect);
    }

    private void OnButtonSelect(SpeciesSelectorButton control, SpeciesSelectorButton.SelectedUIEvent ev)
    {
        _selectedButton?.Deselect();
        _selectedButton = control;
        //TODO apply species change to live profile
    }

    private void OnSpeciesGroupUIAdded(GroupedSpeciesList speciesList, ControlAddedUIEvent ev)
    {
        foreach (var species in _prototypeManager.EnumeratePrototypes<SpeciesPrototype>())
        {
            if (!species.RoundStart)
                continue;
            speciesList.AddSpecies(species, _spriteSystem.Frame0(species.SpeciesIcon), Loc.GetString(species.Name));
        }
    }

    private void OnSpeciesGroupUIRemoved(GroupedSpeciesList speciesList, ControlRemovedUIEvent ev)
    {
        speciesList.ClearSpecies();
    }
}