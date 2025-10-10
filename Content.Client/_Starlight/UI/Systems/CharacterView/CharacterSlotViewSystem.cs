// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.UI.Systems.CharacterView.Controls;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.UIEvents;

namespace Content.Client._Starlight.UI.Systems.CharacterView;

public sealed class CharacterSlotViewSystem : EntitySystem, IUIEventSubscriber
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;

    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    public override void Initialize()
    {
        _uiManager.SubscribeUIEvent<CharacterSlotView, CharacterSlotView.SlotChangedEvent>(this, OnSlotChanged);
        _uiManager.SubscribeUIEvent<CharacterSlotView, ControlEnteredTreeUIEvent>(this, OnPreviewAdded);
        _uiManager.SubscribeUIEvent<CharacterSlotView, ControlExitedTreeUIEvent>(this, OnPreviewRemoved);
    }

    private void OnPreviewRemoved(CharacterSlotView viewControl, ControlExitedTreeUIEvent ev)
    {
        viewControl.LinkedProfileSlot = null;
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

        if (_characterProfileSystem.TryGetCharacterInSlot(slot, out var profile, out var preview))
        {
            viewControl.LinkedProfileSlot = profile;
            viewControl.SetEntity(preview);
            viewControl.LinkedPreview = preview;
        }
        else
        {
            viewControl.LinkedProfileSlot = null;
            viewControl.SetEntity(null);
            viewControl.LinkedPreview = null;
        }
    }
}