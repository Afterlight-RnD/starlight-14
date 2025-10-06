// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfileSystem;


[Serializable, NetSerializable]
public sealed class MsgRequestCharacterProfileDataUpdate(
    NetEntity profileEnt,
    NetEntity preferencesEnt,
    CharacterProfileData? profileData,
    int slot) : EntityEventArgs
{
    public int Slot = slot;
    public NetEntity PreferencesEnt = preferencesEnt;
    public NetEntity ProfileEnt = profileEnt;
    public CharacterProfileData? Data = profileData;
}

[Serializable, NetSerializable]
public sealed class MsgReceiveUpdatedCharacterProfile(int slot,NetEntity profileEnt) : EntityEventArgs
{
    public int Slot = slot;
    public NetEntity ProfileEnt = profileEnt;
}

[ByRefEvent]
public record struct CharacterProfileUpdatedEvent(Entity<CharacterProfileComponent> ProfileEnt);

[ByRefEvent]
public record struct ValidateCharacterProfileEvent(CharacterProfileData ProfileData, bool InvalidData = false);