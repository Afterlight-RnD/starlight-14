// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared.Preferences;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles.Data;

[Serializable, NetSerializable]
public sealed partial class LegacyCharacterData : CharacterData
{
    [DataField] public HumanoidCharacterProfile LegacyProfile = HumanoidCharacterProfile.DefaultWithSpecies();
}