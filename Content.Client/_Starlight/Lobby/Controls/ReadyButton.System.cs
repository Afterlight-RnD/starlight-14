// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Content.Client.GameTicking.Managers;
using Content.Client.Lobby;
using Robust.Client.Console;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.Lobby.Controls;

public sealed class ReadyButtonSystem : BoundUISystem<ReadyButton>
{
    [Dependency] private readonly IClientConsoleHost _consoleHost = default!;
    [Dependency] private readonly ClientGameTicker _gameTicker = default!;
    [Dependency] private readonly IClientPreferencesManager _preferences = default!;

    public override void Initialize()
    {
        base.Initialize();
        _gameTicker.LobbyStatusUpdated += OnLobbyStatusUpdated;
    }

    private void OnReadyButtonToggled(ReadyButton control, ref readonly ButtonToggledUIEvent args)
    {
        if (_gameTicker.IsGameStarted)
            return;
        _consoleHost.ExecuteCommand($"toggleready {args.Pressed}");
        control.Refresh(false,true);
    }

    private void OnLobbyStatusUpdated()
    {
        var gameStarted = _gameTicker.IsGameStarted;
        foreach (var readyButton in IterateBoundControls())
        {
            if (gameStarted)
            {
                readyButton.Refresh(gameStarted,(_preferences.Preferences?.JobPrioritiesFiltered().Count ?? 0) == 0);
            }
            else
            {
                readyButton.Refresh(gameStarted,false);
            }
        }
    }


    protected override void BoundControlEnteredTree(ReadyButton boundControl)
    {
        boundControl.Ready.OnToggled += HandleReadyToggled;
    }

    private void HandleReadyToggled(BaseButton.ButtonToggledEventArgs args)
    {
        if (_gameTicker.IsGameStarted)
            return;
        _consoleHost.ExecuteCommand($"toggleready {args.Pressed}");
        foreach (var readyButton in IterateBoundControls())
        {
            readyButton.Refresh(false,true);
        }
    }

    protected override void BoundControlExitedTree(ReadyButton boundControl)
    {
    }
}