// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.UI.Core;

namespace Content.Client._Starlight.CharacterProfiles.UI;

public sealed class CharacterSlotViewSystem : BoundUISystem<CharacterSlotView>
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeUIEvent<CharacterSlotView,CharacterSlotView.SlotChangedUIEvent>(OnSlotChanged);
    }

    protected override void BoundControlEnteredTree(CharacterSlotView boundControl)
    {
        UpdateLinkedEntity(boundControl, boundControl.Slot);
    }

    protected override void BoundControlExitedTree(CharacterSlotView boundControl)
    {
        boundControl.SetEntity(null);
    }

    private void OnSlotChanged(CharacterSlotView viewControl, ref readonly CharacterSlotView.SlotChangedUIEvent ev)
    {
        UpdateLinkedEntity(viewControl, viewControl.Slot);
    }

    private void UpdateLinkedEntity(CharacterSlotView viewControl, int slot)
    {
        if (_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
        {
            var preview = _characterProfileSystem.EnsurePreviewEntity(slot, profile);
            viewControl.SetEntity(preview);
        }
        else
        {
            viewControl.SetEntity(null);
        }
    }
}