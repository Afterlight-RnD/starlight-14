// SPDX-FileCopyrightText: 2025 Afterlight RnD
// SPDX-License-Identifier: ASL-1.0

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.Lobby;

public interface ILobbyPlayerBarControl
{
    public Control GetControl() => (Control)this;
}
