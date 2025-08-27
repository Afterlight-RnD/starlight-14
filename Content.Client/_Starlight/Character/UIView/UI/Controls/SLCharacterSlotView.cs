// SPDX-FileCopyrightText: 2025 Afterlight RnD
// SPDX-License-Identifier: ASL-1.2

using Content.Client._Starlight.Character.ProfileSlots;
using Robust.Client.UserInterface.UIEvents;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._Starlight.Character.UIView.UI.Controls;

public sealed class SLCharacterSlotView  : SLCharacterView, IUIEventSubscriber
{
    private int _slotIdx = -1;

    public int SlotIdx
    {
        get => _slotIdx;
        set
        {
            if (_slotIdx == value)
                return;
            _slotIdx = value;
            if (_slotIdx < 0)
            {
                ClearCharacter();
                _slotIdx = -1;
                return;
            }
            SetFromProfile(CharacterViewSystem.GetProfileForSlot(_slotIdx));
        }
    }

    public SLCharacterSlotView()
    {
        IoCManager.InjectDependencies(this);
    }

    protected override void EnteredTree()
    {
        UserInterfaceManager.SubscribeGlobalUIEvent<CharacterSlotUpdatedUIEvent>(this,OnSlotUpdatedEvent);
    }

    private void OnSlotUpdatedEvent(CharacterSlotUpdatedUIEvent ev)
    {
        if (_slotIdx != ev.SlotIdx)
            return;
        SetFromProfile(ev.Profile);
    }
}