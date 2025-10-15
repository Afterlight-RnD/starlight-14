// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles.Data;

[Serializable, NetSerializable]
public sealed partial class CharacterRolePreferences : CharacterData
{
    [DataField] public Dictionary<ProtoId<AntagPrototype>, RoleLoadout> AntagLoadouts = new();
    [DataField] public HashSet<ProtoId<AntagPrototype>> EnabledAntags = new();

    [DataField] public Dictionary<ProtoId<JobPrototype>, RoleLoadout> JobLoadouts = new();
    [DataField] public HashSet<ProtoId<JobPrototype>> EnabledJobs = new();
}