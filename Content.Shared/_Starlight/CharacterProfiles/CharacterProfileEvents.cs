// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles;

public record struct CharacterProfileApplyEvent(EntityUid Target, CharacterProfile Profile, bool IsDoll = false);

public record struct CharacterProfileRandomizeEvent(CharacterProfile Profile);
// == NetEvents ==

[Serializable, NetSerializable]
public sealed class MsgUpdateCharacterProfile(int slot, List<CharacterData> data) : EntityEventArgs
{
    public int Slot = slot;
    public List<CharacterData> Data = data;

    public MsgUpdateCharacterProfile(int slot, CharacterProfile profile, bool onlyDirty = false) : this(slot,
        profile.GetData(onlyDirty))
    {
        profile.ClearDirty();
    }
}

[Serializable, NetSerializable]
public sealed class MsgSyncCharacterProfile(int slot, List<CharacterData> data, bool partialData = true) : EntityEventArgs
{
    public bool PartialData = partialData;
    public int Slot = slot;
    public List<CharacterData> Data = data;

    public MsgSyncCharacterProfile(int slot, CharacterProfile profile, bool onlyDirty = false) : this(slot,
        profile.GetData(onlyDirty), false)
    {
        profile.ClearDirty();
    }
}

[Serializable, NetSerializable]
public sealed class MsgDeleteCharacterProfile(int slot) : EntityEventArgs
{
    public int Slot = slot;
}