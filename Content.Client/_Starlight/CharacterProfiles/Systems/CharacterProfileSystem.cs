// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.CharacterProfileSystem.Systems;
using Robust.Client.Player;
using Robust.Shared.Network;

namespace Content.Client._Starlight.CharacterProfiles.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class CharacterProfileSystem : SharedCharacterProfileSystem
{
    public override void Initialize()
    {
        SubscribeLocalEvent<CharacterProfileComponent, AfterAutoHandleStateEvent>(OnReceivedUpdatedProfile);
    }

    private void OnReceivedUpdatedProfile(Entity<CharacterProfileComponent> ent, ref AfterAutoHandleStateEvent args)
    {
        var ev = new CharacterProfileUpdatedEvent(ent);
        RaiseLocalEvent(ref ev);
    }

    public void ApplyProfileChanges(Entity<CharacterProfileComponent> target)
    {
        RaiseNetworkEvent(new CharacterProfileDataUpdateRequest(GetNetEntity(target), target.Comp.Data, target.Comp.Slot));
    }
}