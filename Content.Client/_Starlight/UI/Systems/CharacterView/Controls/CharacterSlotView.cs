// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Systems.CharacterView.Controls;

public sealed class CharacterSlotView : SpriteView, IUIEventSubscriber
{
    private int _slot = -1;

    public CharacterProfile? LinkedProfile = null;

    [ViewVariables]
    public int Slot
    {
        get => _slot;
        set
        {
            if (_slot == value)
                return;
            _slot = value;
        }
    }

    protected override void EnteredTree()
    {
    }

    protected override void ExitedTree()
    {
    }

    private void OnCharacterUpdated(CharacterProfileUpdatedUIEvent ev)
    {
        if (LinkedProfile?.Slot != _slot)
            return;
        LinkedProfile = ev.Profile;
    }

    private void OnCharacterDeleted(CharacterProfileDeletedUIEvent ev)
    {
        if (LinkedProfile?.Slot != _slot)
            return;
        LinkedProfile = null;
        SetEntity(null);
    }

    private void OnCharacterCreated(CharacterProfileCreatedUIEvent ev)
    {
        if (LinkedProfile?.Slot != _slot)
            return;
        LinkedProfile = ev.Profile;
        SetEntity(ev.PreviewEntity);
    }



    public record struct SlotChangedEvent();
}