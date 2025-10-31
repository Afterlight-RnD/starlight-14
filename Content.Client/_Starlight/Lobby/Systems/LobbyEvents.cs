// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.Audio;
using Robust.Client.Graphics;

namespace Content.Client._Starlight.Lobby;



//UI Events
public record struct LobbyUpdatedUIEvent(
    TimeSpan StartTime,
    TimeSpan RoundStartTimeSpan,
    bool IsGameStarted,
    bool Paused,
    Texture? BackgroundTexture,
    bool PlayerReadied);

public record struct LobbyMusicUpdatedUIEvent(AudioStream? Song)
{
    public bool HasSong => Song != null;
    public string Artist => Song!.Artist!;
    public string Title => Song!.Title!;
    public string Name => Song!.Name!;
};