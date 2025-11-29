// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.Audio;
using Robust.Client.Graphics;

namespace Content.Client._Starlight.Lobby;
public record struct LobbyInfoUpdatedUIEvent(string? NewInfo);

public record struct RoundStateChangedUIEvent(bool Started, bool Paused);

public record struct PlayerReadyStatusChangedUIEvent(bool NewReady);

public record struct RoundStartTimeChangedUIEvent(TimeSpan StartTime);

public record struct LobbyMusicUpdatedUIEvent(AudioStream? Song)
{
    public bool HasSong => Song != null;
    public string Artist => Song!.Artist!;
    public string Title => Song!.Title!;
    public string Name => Song!.Name!;
};