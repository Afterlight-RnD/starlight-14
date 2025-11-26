// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

/// <summary>
/// Event raised when UIScreens are changed
/// </summary>
/// <param name="Old">Previous UIScreen</param>
/// <param name="New">New UIScreen</param>
public record struct ScreenChangedUIEvent(UIScreen? Old, UIScreen? New);

public record struct ControlEnteredTreeUIEvent;
public record struct ControlExitedTreeUIEvent;