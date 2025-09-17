// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Shared.GameStates;
using Robust.Shared.Network;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfileSystem.Components;


/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState(raiseAfterAutoHandleState:true)]
public sealed partial class CharacterProfileComponent : Component
{
    public override bool SessionSpecific => true;

    /// <summary>
    /// Entity that provides the sprite preview for this profile. This will always be null on the server
    /// </summary>
    public Entity<HumanoidAppearanceComponent>? PreviewEntity = null;

    [DataField, AutoNetworkedField] public int Slot = -1;

    [DataField, AutoNetworkedField] public CharacterProfileData Data;

    [DataField(serverOnly:true)] public NetUserId? OwnerNetId = null;
}

[Serializable, NetSerializable]
public sealed partial class CharacterProfileData
{
    [DataField] public HumanoidCharacterProfile? Profile;
}