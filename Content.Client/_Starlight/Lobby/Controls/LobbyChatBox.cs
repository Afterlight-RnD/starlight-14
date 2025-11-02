// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client.UserInterface.Systems.Chat.Widgets;

namespace Content.Client._Starlight.Lobby.Controls;

public sealed class LobbyChatBox : ChatBox; //Need this because UIWidgets are singletons indexed by type. Probably should have implemented something like a "named widget" system instead...