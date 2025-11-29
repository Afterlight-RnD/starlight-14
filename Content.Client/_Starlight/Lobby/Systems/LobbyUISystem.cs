// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Content.Client.Audio;
using Content.Client.GameTicking.Managers;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.Lobby.Systems;

public sealed class LobbySystem : BoundUISystem<SLDummyLobbyControl>
{
    [Dependency] private readonly IResourceCache _resourceCache = default!;
    [Dependency] private readonly ClientGameTicker _gameTicker = default!;
    [Dependency] private readonly ContentAudioSystem _audioSystem = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    private bool _gameStarted = false;
    private bool _gamePaused = false;
    private TimeSpan _startTime;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        _gameTicker.LobbyStatusUpdated += OnLobbyStatusUpdated;
        _audioSystem.LobbySoundtrackChanged += OnLobbySoundtrackChanged;
        _gameTicker.InfoBlobUpdated += OnInfoBlobUpdated;
    }

    protected override void BoundControlEnteredTree(SLDummyLobbyControl boundControl)
    {
        if (_gameTicker.LobbyBackground != null)
        {
            var lobbyProto = _prototypeManager.Index(_gameTicker.LobbyBackground);
            Texture? lobbyBackground = _resourceCache.GetResource<TextureResource>(lobbyProto.Background);
            boundControl.ChangeLobbyBackground(lobbyBackground);
        }
    }

    protected override void BoundControlExitedTree(SLDummyLobbyControl boundControl)
    {

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
        {
            var lobbyProto = _prototypeManager.Index(_gameTicker.LobbyBackground);
            lobbyBackground = _resourceCache.GetResource<TextureResource>(lobbyProto.Background);
        }
        foreach (var lobbyControl in IterateBoundControls())
        {
            lobbyControl.ChangeLobbyBackground(lobbyBackground);
        }
        RaiseUIEvent(new RoundStateChangedUIEvent(_gameTicker.IsGameStarted, _gameTicker.Paused));

        // RaiseUIEvent(new LobbyUpdatedUIEvent(_gameTicker.StartTime, _gameTicker.RoundStartTimeSpan, _gameTicker.IsGameStarted, _gameTicker.Paused, lobbyBackground, _gameTicker.AreWeReady));
    }
}