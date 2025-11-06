// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Content.Client.Audio;
using Content.Client.GameTicking.Managers;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;

namespace Content.Client._Starlight.Lobby.Systems;

public sealed class LobbySystem : UISystem
{
    [Dependency] private readonly IResourceCache _resourceCache = default!;
    [Dependency] private readonly ClientGameTicker _gameTicker = default!;
    [Dependency] private readonly ContentAudioSystem _audioSystem = default!;

    private bool _gameStarted = false;
    private bool _gamePaused = false;
    private TimeSpan _startTime;

    /// <inheritdoc/>
    public override void Initialize()
    {
        _gameTicker.LobbyStatusUpdated += OnLobbyStatusUpdated;
        _audioSystem.LobbySoundtrackChanged += OnLobbySoundtrackChanged;
        _gameTicker.InfoBlobUpdated += OnInfoBlobUpdated;
    }

    private void OnInfoBlobUpdated()
    {
        RaiseUIEvent(new LobbyInfoUpdatedUIEvent(_gameTicker.ServerInfoBlob));
    }

    private void OnLobbySoundtrackChanged(LobbySoundtrackChangedEvent args)
    {
        if (args.SoundtrackFilename == null
            || !_resourceCache.TryGetResource<AudioResource>(args.SoundtrackFilename, out var lobbySongResource))
        {
            RaiseUIEvent(new LobbyMusicUpdatedUIEvent(null));
            return;
        }
        RaiseUIEvent(new LobbyMusicUpdatedUIEvent(lobbySongResource.AudioStream));
    }

    private void OnLobbyStatusUpdated()
    {
        Texture? lobbyBackground = null;
        if (_gameTicker.LobbyBackground != null)
            lobbyBackground = _resourceCache.GetResource<TextureResource>(_gameTicker.LobbyBackground);

        RaiseUIEvent(new LobbyBackgroundChangedUIEvent(lobbyBackground));
        RaiseUIEvent(new RoundStateChangedUIEvent(_gameTicker.IsGameStarted, _gameTicker.Paused));

        // RaiseUIEvent(new LobbyUpdatedUIEvent(_gameTicker.StartTime, _gameTicker.RoundStartTimeSpan, _gameTicker.IsGameStarted, _gameTicker.Paused, lobbyBackground, _gameTicker.AreWeReady));
    }
}