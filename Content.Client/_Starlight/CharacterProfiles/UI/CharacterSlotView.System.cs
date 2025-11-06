// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.UI.Core;

namespace Content.Client._Starlight.CharacterProfiles.UI;

public sealed class CharacterSlotViewSystem : UISystem
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;
    public override void Initialize()
    {
        SubscribeUIEvent<CharacterSlotView,CharacterSlotView.SlotChangedUIEvent>(OnSlotChanged);
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