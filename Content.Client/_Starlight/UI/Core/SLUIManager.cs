// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

public sealed class SLUIManager : IPostInjectInit
{
    [Dependency] private UIEventBus _uiEventBus = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;

    public UIEventBus UIEventBus => _uiEventBus;

    public void PostInject()
    {
        _uiManager.OnScreenChanged += OnUIScreenChanged;
    }

    private void OnUIScreenChanged((UIScreen? Old, UIScreen? New) obj)
    {
        if (obj.New == null) return;
        _uiEventBus.RaiseEvent(new ScreenChangedUIEvent(obj.Old, obj.New));
    }
}