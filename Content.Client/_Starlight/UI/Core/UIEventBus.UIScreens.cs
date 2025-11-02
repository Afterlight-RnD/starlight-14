// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

public sealed partial class UIEventBus
{
    private void OnUIScreenChanged((UIScreen? Old, UIScreen? New) obj)
    {
        if (obj.New == null) return;
        RaiseEvent(new ScreenChangedUIEvent(obj.Old, obj.New));
    }
}