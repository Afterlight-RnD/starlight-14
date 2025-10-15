// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles;

public record struct ApplyCharacterProfileEvent(EntityUid Target, ICharacterData Data, bool IsDoll);

// == NetEvents ==

[Serializable, NetSerializable]
public sealed class MsgUpdateCharacterProfile(int slot, List<ICharacterData> data) : EntityEventArgs
{
    public int Slot = slot;
    public List<ICharacterData> Data = data;
}

[Serializable, NetSerializable]
public sealed class MsgLoadCharacterProfile(int slot, List<ICharacterData> data) : EntityEventArgs
{
    public int Slot = slot;
    public List<ICharacterData> Data = data;
}

[Serializable, NetSerializable]
public sealed class MsgDeleteCharacterProfile(int slot) : EntityEventArgs
{
    public int Slot = slot;
}


[Serializable, NetSerializable]
public sealed class MsgCreateCharacterProfile(int slot) : EntityEventArgs
{
    public int Slot = slot;
}