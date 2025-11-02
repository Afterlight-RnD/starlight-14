// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Content.Client.GameTicking.Managers;
using Robust.Shared.Timing;

namespace Content.Client._Starlight.Lobby.Widgets;

public sealed class LobbyInfoWidgetSystem : UISystem<LobbyInfoWidget>
{
    [Dependency] private readonly IGameTiming _gameTiming = default!;
    [Dependency] private readonly ClientGameTicker _gameTicker = default!;


    public override void Initialize()
    {
        _gameTicker.InfoBlobUpdated += OnInfoBlobUpdated;
    }

    private void OnInfoBlobUpdated()
    {
        foreach (var infoWidget in IterateInstances())
            infoWidget.UpdateInfoBlob(_gameTicker.ServerInfoBlob);
    }

    public override void Update(float frameTime)
    {
        foreach (var instances in IterateInstances())
            instances.UpdateTimer(_gameTicker.IsGameStarted, _gameTiming.CurTime, _gameTicker.RoundStartTimeSpan);
    }
}