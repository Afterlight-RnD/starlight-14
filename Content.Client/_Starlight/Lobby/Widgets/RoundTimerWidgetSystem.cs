// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Content.Client.GameTicking.Managers;
using Robust.Shared.Timing;

namespace Content.Client._Starlight.Lobby.Widgets;

public sealed class RoundTimerWidgetSystem : UISystem<RoundTimerWidget>
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;

    [Dependency] private readonly ClientGameTicker _gameTicker = default!;

    public override void Update(float frameTime)
    {
        foreach (var timer in IterateInstances())
        {
            timer.Update(_gameTicker.IsGameStarted, _gameTicker.Paused,
                _gameTicker.RoundStartTimeSpan, _gameTiming.CurTime,
                _gameTicker.StartTime);
        }
    }
}