// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Humanoid;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.UIEvents;

namespace Content.Client._Starlight.UI.Systems.CharacterView.Controls;

public sealed class CharacterSlotView : SpriteView, IUIEventSubscriber
{
    private int _slot = -1;

    public Entity<CharacterProfileComponent>? LinkedProfileSlot = null;

    public Entity<SpriteComponent, HumanoidAppearanceComponent>? LinkedPreview = null;

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
    public record struct SlotChangedEvent();
}