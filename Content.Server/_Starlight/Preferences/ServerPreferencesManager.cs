// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Content.Shared.Preferences;
using Robust.Shared.Player;

namespace Content.Server.Preferences.Managers;

public sealed partial class ServerPreferencesManager
{
    [Dependency] private readonly IEntityManager _entMan = default!;

    private void SLFinishLoad(ICommonSession session, PlayerPreferences preferences)
    {
        _entMan.EventBus.RaiseEvent(EventSource.Local, new PlayerPreferencesLoadedEvent(session, preferences));
    }

    private void SLUnload(ICommonSession session)
    {
        _entMan.EventBus.RaiseEvent(EventSource.Local, new PlayerPreferencesUnloadedEvent(session));
    }

    public async void SLSaveCharacter(ICommonSession session, int slot, CharacterProfile profile)
    {
        //TODO: save new character profile data
    }

    public async void SLDeleteCharacter(ICommonSession session, int slot)
    {
        try
        {
            await DeleteProfile(session.UserId, slot);
        }
        catch (Exception e)
        {
            _sawmill.Error(e.Message);
        }
    }
}