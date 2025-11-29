// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Core;

public sealed class SLUIManager : IPostInjectInit
{
    [Dependency] private UIEventBus _uiEventBus = default!;
    [Dependency] protected readonly IUserInterfaceManager UiManager = default!;

    public UIEventBus UIEventBus => _uiEventBus;

    public void PostInject()
    {
        UiManager.OnScreenChanged += OnUIScreenChanged;
    }

    private void OnUIScreenChanged((UIScreen? Old, UIScreen? New) obj)
    {
        if (obj.New == null) return;
        _uiEventBus.RaiseEvent(new ScreenChangedUIEvent(obj.Old, obj.New));
    }
}

public static class UIManagerExtensions
{
    public static bool TryGetActiveWidget<TWidget>(this IUserInterfaceManager uiManager,[NotNullWhen(true)] out TWidget? widget) where TWidget : UIWidget, new()
    {
        widget = uiManager.GetActiveUIWidgetOrNull<TWidget>();
        return widget != null;
    }
}