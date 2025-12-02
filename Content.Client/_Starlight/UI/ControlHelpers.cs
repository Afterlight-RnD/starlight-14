// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI;

public static class ControlHelper
{
    public static T SaveTo<T>(this T control, Action<T> action) where T : Control
    {
        action(control);
        return control;
    }
    public static TextureButton OnClick(this TextureButton button, Action action)
    {
        button.OnPressed += _ => action();
        return button;
    }

    public static TControl? FindParentOfTypeRecursive<TControl>(this Control control)
    where TControl: Control
    {
        switch (control)
        {
            case null:
                return null;
            case TControl found:
                return found;
            default:
                return control.Parent != null ? FindParentOfTypeRecursive<TControl>(control.Parent) : null;
        }
    }
}
