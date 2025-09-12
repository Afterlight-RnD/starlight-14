// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Systems.CharacterView.Controls;

public sealed class CharacterSlotView : SpriteView
{
    private int _slot = -1;

    public int Slot
    {
        get => _slot;
        set
        {
            if (_slot == value)
                return;
            var prevSlot = _slot;
            _slot = value;
            UserInterfaceManager.RaiseUIEvent(this, new CharacterViewSlotChangedUIEvent(prevSlot));
        }
    }
}

public record struct CharacterViewSlotChangedUIEvent(int PreviousSlot);