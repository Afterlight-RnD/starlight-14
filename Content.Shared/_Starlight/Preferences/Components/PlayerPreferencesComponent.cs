// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.Preferences.Components;

/// <summary>
/// This stores player preference data
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
public sealed partial class PlayerPreferencesComponent : Component
{
    [DataField(serverOnly:true)] public NetUserId? OwnerNetId = null;

    [DataField(serverOnly: true)] public bool Loaded = false;

    [DataField, AutoNetworkedField] public Dictionary<int,EntityUid> CharacterProfiles = new();

    [DataField, AutoNetworkedField] public PlayerRolePreferences JobPreferences = new();
}

[DataDefinition, Serializable, NetSerializable]
public sealed partial class PlayerRolePreferences
{
    [DataField] public ProtoId<JobPrototype>? High;
    [DataField]  public HashSet<ProtoId<JobPrototype>>? Medium ;
    [DataField]  public HashSet<ProtoId<JobPrototype>>? Low;
    [DataField]  public HashSet<ProtoId<JobPrototype>> ?Never;
};