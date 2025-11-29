// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Content.Client._Starlight.UI.Core;

namespace Content.Client._Starlight.CharacterProfiles.UI;

public sealed class CharacterSlotView : SLSpriteView
{
    private int _slot = -1;

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
        base.EnteredTree();
        SubscribeUIEvent<CharacterSlotUpdatedUIEvent>(OnCharacterUpdated);
    }

    private void OnCharacterUpdated(ref readonly CharacterSlotUpdatedUIEvent ev)
    {
        if (ev.Slot != _slot)
            return;
        SetEntity(ev.Preview);
    }

    public record struct SlotChangedUIEvent();
}