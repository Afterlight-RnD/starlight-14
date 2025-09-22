// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfileSystem;


[Serializable, NetSerializable]
public sealed class CharacterProfileDataUpdateRequest( NetEntity profileEnt,  CharacterProfileData? profileData, int slot) : EntityEventArgs
{
    public int Slot = slot;
    public NetEntity ProfileEnt = profileEnt;
    public CharacterProfileData? Data = profileData;
}

[Serializable, NetSerializable]
public sealed class ReceiveUpdatedCharacterProfileEvent(int slot,NetEntity profileEnt) : EntityEventArgs
{
    public int Slot = slot;
    public NetEntity ProfileEnt = profileEnt;
}

[ByRefEvent]
public record struct CharacterProfileUpdatedEvent(Entity<CharacterProfileComponent> ProfileEnt);

[ByRefEvent]
public record struct ValidateCharacterProfileEvent(CharacterProfileData ProfileData, bool InvalidData = false);