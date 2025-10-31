// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Core;

/// <summary>
/// Event raised when a control has entered the UI Tree.
/// NOTE: to function, you must manually raise this event in the target control
/// </summary>
/// <param name="Control">Control beind added</param>
/// <typeparam name="TControl"></typeparam>
public record struct ControlAddedUIEvent<TControl>(TControl Control) where TControl : Control, IUIEventDispatcher,new();

/// <summary>
/// Event raised when a control has exits the UI Tree.
/// NOTE: to function, you must manually raise this event in the target control
/// </summary>
/// <param name="Control">Control beind added</param>
/// <typeparam name="TControl"></typeparam>
public record struct ControlRemovedUIEvent<TControl>(TControl Control) where TControl : Control, IUIEventDispatcher,new();

/// <summary>
/// Event raised when a control has exits the UI Tree.
/// NOTE: to function, you must manually raise this event in the target control
/// </summary>
/// <param name="Control">Control beind added</param>
/// <typeparam name="TControl"></typeparam>
public record struct ControlVisibilityChangedUIEvent<TControl>(TControl Control, bool NewVisability) where TControl : Control, IUIEventDispatcher,new();

/// <summary>
/// Event raised when a button has been pressed.
/// NOTE: to function, you must manually raise this event in the target control
/// </summary>
/// <param name="Button">Control being pressed</param>
/// <typeparam name="TButton"></typeparam>
public record struct ButtonPressedUIEvent<TButton>(TButton Button) where TButton : BaseButton, IUIEventDispatcher,new();

/// <summary>
/// Event raised when a button has been toggled.
/// NOTE: to function, you must manually raise this event in the target control
/// </summary>
/// <param name="Button">Control being toggled</param>
/// <typeparam name="TButton"></typeparam>
public record struct ButtonToggledUIEvent<TButton>(TButton Button, bool ButtonState) where TButton : BaseButton, IUIEventDispatcher,new();