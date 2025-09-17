// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.UIEvents;

namespace Content.Client._Starlight.UI.Systems.CharacterView.Controls;

public sealed class CharacterSlotView : SpriteView, IUIEventSubscriber
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
            UserInterfaceManager.RaiseUIEvent(this, new RefreshProfilePreviewUIEvent());
        }
    }

    protected override void EnteredTree()
    {
        base.EnteredTree();
        UserInterfaceManager.SubscribeGlobalUIEvent<CharacterProfileUpdatedUIEvent>(this, OnCharacterProfileUpdated);
        UserInterfaceManager.RaiseUIEvent(this, new RefreshProfilePreviewUIEvent());
    }

    private void OnCharacterProfileUpdated(CharacterProfileUpdatedUIEvent ev)
    {
        if (ev.Slot != Slot)
            return;
        UserInterfaceManager.RaiseUIEvent(this, new RefreshProfilePreviewUIEvent());
    }

    protected override void ExitedTree()
    {
        base.ExitedTree();
        UserInterfaceManager.UnSubscribeAllUIEvents(this);
    }
}

public record struct RefreshProfilePreviewUIEvent();