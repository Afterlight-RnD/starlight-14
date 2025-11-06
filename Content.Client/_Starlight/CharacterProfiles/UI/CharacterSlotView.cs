// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.CharacterProfiles.UI;

public sealed class CharacterSlotView : SpriteView
{
    private int _slot = -1;
    private UIEventHandle _profileUpdatedEvent;

    [ViewVariables]
    public int Slot
    {
        get => _slot;
        set
        {
            if (_slot == value)
                return;
            _slot = value;
            UIEvents.RaiseControlEvent(this, new SlotChangedUIEvent());
        }
    }
    protected override void EnteredTree()
    {
        _profileUpdatedEvent = UIEvents.Subscribe<CharacterSlotUpdatedUIEvent>(OnCharacterUpdated);
    }

    protected override void ExitedTree()
    {
        _profileUpdatedEvent.Unsubscribe();
    }

    private void OnCharacterUpdated(ref readonly CharacterSlotUpdatedUIEvent ev)
    {
        if (ev.Slot != _slot)
            return;
        SetEntity(ev.Preview);
    }

    public record struct SlotChangedUIEvent();
}