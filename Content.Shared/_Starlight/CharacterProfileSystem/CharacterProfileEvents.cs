// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfileSystem;


[Serializable, NetSerializable]
public sealed class CharacterProfileDataUpdateRequest( NetEntity profileEnt,  CharacterProfileData profileData, int slot) : EntityEventArgs
{
    public int Slot = slot;
    public NetEntity ProfileEnt = profileEnt;
    public CharacterProfileData Data = profileData;
}

[ByRefEvent]
public record struct CharacterProfileUpdatedEvent(Entity<CharacterProfileComponent> ProfileEnt);

[ByRefEvent]
public record struct ValidateCharacterProfileEvent(CharacterProfileData ProfileData, bool InvalidData = false);