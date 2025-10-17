// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.UIEvents;

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
            UserInterfaceManager.RaiseUIEvent(this, new SlotChangedEvent());
        }
    }

    protected override void EnteredTree()
    {
        UserInterfaceManager.SubscribeUIEvent<CharacterProfileCreatedUIEvent>(this,OnCharacterCreated);
        UserInterfaceManager.SubscribeUIEvent<CharacterProfileUpdatedUIEvent>(this,OnCharacterUpdated);
        UserInterfaceManager.SubscribeUIEvent<CharacterProfileDeletedUIEvent>(this,OnCharacterDeleted);
    }

    protected override void ExitedTree()
    {
        UserInterfaceManager.UnSubscribeAllUIEvents(this);
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