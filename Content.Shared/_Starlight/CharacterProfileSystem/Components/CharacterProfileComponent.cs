// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfileSystem.Components;


/// <summary>
/// Stores Character profile data
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(raiseAfterAutoHandleState:true)]
public sealed partial class CharacterProfileComponent : Component
{
    [DataField("data"), AutoNetworkedField]
    private CharacterProfileData? _data = null;
    //Serialization kludge for copying, this should never be null
    public CharacterProfileData Data { get => _data!; set => _data = value; }

    [DataField, AutoNetworkedField] public int Slot = -1;

    [DataField(serverOnly:true)] public NetUserId? OwnerNetId = null;

    [DataField, AutoNetworkedField] public Dictionary<ProtoId<AntagPrototype>, RoleLoadout> AntagLoadouts = new();
    [DataField, AutoNetworkedField] public HashSet<ProtoId<AntagPrototype>> EnabledAntags = new();

    [DataField, AutoNetworkedField] public Dictionary<ProtoId<JobPrototype>, RoleLoadout> JobLoadouts = new();
    [DataField, AutoNetworkedField] public HashSet<ProtoId<JobPrototype>> EnabledJobs = new();

    [DataField, AutoNetworkedField] public ProtoId<JobPrototype> FavoriteJob = "";

    [DataField] public EntityUid? Doll = null;
}

[DataRecord, Serializable, NetSerializable]
public sealed partial record CharacterProfileData(HumanoidCharacterProfile Profile);
