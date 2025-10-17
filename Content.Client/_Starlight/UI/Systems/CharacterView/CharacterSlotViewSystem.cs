// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.UI.Systems.CharacterView.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.UIEvents;

namespace Content.Client._Starlight.UI.Systems.CharacterView;

public sealed class CharacterSlotViewSystem : UISystem
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;

    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    public override void Initialize()
    {
        SubscribeUIEvent<CharacterSlotView, CharacterSlotView.SlotChangedEvent>(OnSlotChanged);
        SubscribeUIEvent<CharacterSlotView, ControlEnteredTreeUIEvent>(OnPreviewAdded);
        SubscribeUIEvent<CharacterSlotView, ControlExitedTreeUIEvent>(OnPreviewRemoved);
    }

    private void OnPreviewRemoved(CharacterSlotView viewControl, ControlExitedTreeUIEvent ev)
    {
        viewControl.LinkedProfile = null;
        viewControl.SetEntity(null);
    }

    private void OnPreviewAdded(CharacterSlotView viewControl, ControlEnteredTreeUIEvent ev)
    {
        UpdateLinkedEntity(viewControl, viewControl.Slot);
    }

    private void OnSlotChanged(CharacterSlotView viewControl, CharacterSlotView.SlotChangedEvent ev)
    {
        UpdateLinkedEntity(viewControl, viewControl.Slot);
    }

    private void UpdateLinkedEntity(CharacterSlotView viewControl, int slot)
    {
        if (_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
        {
            var preview = _characterProfileSystem.EnsurePreviewEntity(slot, profile);
            viewControl.LinkedProfile = profile;
            viewControl.SetEntity(preview);
        }
        else
        {
            viewControl.LinkedProfile = null;
            viewControl.SetEntity(null);
        }
    }
}