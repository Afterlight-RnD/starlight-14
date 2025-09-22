// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfileSystem.Components;


/// <summary>
/// Stores Character profile data
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(raiseAfterAutoHandleState:true)]
public sealed partial class CharacterProfileComponent : Component
{
    [DataField(required:true), AutoNetworkedField]
    public CharacterProfileData Data;

    [DataField, AutoNetworkedField] public int Slot = -1;

    [DataField(serverOnly:true)] public NetUserId? OwnerNetId = null;
}

[Serializable, NetSerializable]
public sealed partial class CharacterProfileData(HumanoidCharacterProfile profile)
{
    [DataField(required:true)] public HumanoidCharacterProfile Profile = profile;
}