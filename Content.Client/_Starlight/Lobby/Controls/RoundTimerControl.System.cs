// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Content.Client.GameTicking.Managers;

namespace Content.Client._Starlight.Lobby.Controls;

public sealed class RoundTimerControlSystem : BoundUISystem<RoundTimerControl>
{
    [Dependency] private readonly ClientGameTicker _gameTicker = default!;

    public override void Initialize()
    {
        base.Initialize();
        _gameTicker.LobbyStatusUpdated += OnLobbyStatusUpdated;
    }

    private void OnLobbyStatusUpdated()
    {
        foreach (var timerControl in IterateBoundControls())
            UpdateControl(timerControl);
    }

    private void UpdateControl(RoundTimerControl timerControl)
    {
        timerControl.IsStarted = _gameTicker.IsGameStarted;
        timerControl.IsPaused = _gameTicker.Paused;
        timerControl.RoundStartTime = _gameTicker.StartTime;
        timerControl.RoundStartTimeSpan = _gameTicker.RoundStartTimeSpan;
        timerControl.Refresh();
    }


    protected override void BoundControlEnteredTree(RoundTimerControl boundControl)
    {
        UpdateControl(boundControl);
    }

    protected override void BoundControlExitedTree(RoundTimerControl boundControl)
    {

    }

}