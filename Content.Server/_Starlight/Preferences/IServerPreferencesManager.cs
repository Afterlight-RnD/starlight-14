// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Shared.Player;

namespace Content.Server.Preferences.Managers;
public partial interface IServerPreferencesManager
{
    public void SLSaveCharacter(ICommonSession session, int slot, CharacterProfile profile);

    public void SLDeleteCharacter(ICommonSession session, int slot);
}