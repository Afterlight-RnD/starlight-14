// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Controls;

[Virtual]
public class ControlStack : Control
{
    public bool OnlyTopVisible { get; set; } = true;

    public Control? TopControl { get; private set; } = null;
    public ControlStack()
    {
        HorizontalExpand = true;
        VerticalExpand = true;
        HorizontalAlignment = HAlignment.Stretch;
        VerticalAlignment = VAlignment.Stretch;
    }

    public void PushVisibleControl<TControl>(TControl control) where TControl: Control, new()
    {
        if (TopControl == control)
            return;
        var oldTop = TopControl;
        if (!Children.Contains(control))
        {
            AddChild(control);
            control.SetPositionFirst();
        }
        control.Visible = true;
        if (OnlyTopVisible && oldTop != null)
            oldTop.Visible = false;
    }

    public void PopVisibleControl()
    {
        if (TopControl == null)
            return;
        var nextTop = TopControl.GetPositionInParent()-1;
        if (nextTop < 0)
        {
            TopControl = null;
            return;
        }
        if (OnlyTopVisible)
            TopControl.Visible = false;
        TopControl = GetChild(nextTop);
        TopControl.Visible = true;
    }

    protected override void ChildAdded(Control newChild)
    {
        if (newChild.Visible)
        {
            if (OnlyTopVisible && TopControl != null)
                TopControl.Visible = false;
            TopControl = newChild;
        }
        base.ChildAdded(newChild);
    }

    protected override void ChildRemoved(Control child)
    {
        base.ChildRemoved(child);
        if (!child.Visible) // if child is not visible then it can't be the top control
            return;
        if (child == TopControl)
            PopVisibleControl();
    }
}