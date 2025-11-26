// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI;

public static class SLControlExtensions
{
    public static BaseButton WhenPressed(this BaseButton parent, Action<BaseButton.ButtonEventArgs> OnPressed,
        Action<IDisposable>? subscription = null)
    {
        parent.OnPressed += OnPressed;
        subscription?.Invoke(new SLButtonSubscription(() => parent.OnPressed -= OnPressed));
        return parent;
    }

    public static BaseButton WhenMouseEntered(this BaseButton parent, Action<GUIMouseHoverEventArgs> OnPressed,
        Action<IDisposable>? subscription = null)
    {
        parent.OnMouseEntered += OnPressed;
        subscription?.Invoke(new SLButtonSubscription(() => parent.OnMouseEntered -= OnPressed));
        return parent;
    }

    public static BaseButton WhenMouseExited(this BaseButton parent, Action<GUIMouseHoverEventArgs> OnPressed,
        Action<IDisposable>? subscription = null)
    {
        parent.OnMouseExited += OnPressed;
        subscription?.Invoke(new SLButtonSubscription(() => parent.OnMouseEntered -= OnPressed));
        return parent;
    }

    public static Control Add(this Control parent, Control control)
    {
        parent.AddChild(control);
        return parent;
    }

    public static Control Grid(this Control parent, int columns, Action<SLGrid> action)
    {
        var grid = new SLGrid(columns);
        action(grid);
        parent.AddChild(grid);
        return parent;
    }

    public static Control Box(this Control parent, BoxContainer.LayoutOrientation orientation, Action<SLBox> action)
    {
        var select = new SLBox(orientation);
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control Layout(this Control parent, Action<SLLayout> action)
    {
        var select = new SLLayout();
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control TextureRect(this Control parent, Action<SLTextureRect> action)
    {
        var select = new SLTextureRect();
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control LayeredTextureRect(this Control parent, Action<SLLayeredTextureRect> action)
    {
        var select = new SLLayeredTextureRect();
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control Button(this Control parent, Action<SLButton> action)
    {
        var select = new SLButton();
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control Panel(this Control parent, Action<SLPanel> action)
    {
        var select = new SLPanel();
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control Label(this Control parent, Action<SLLabel> action)
    {
        var select = new SLLabel();
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control RichText(this Control parent, Action<SLRichTextLabel> action)
    {
        var select = new SLRichTextLabel();
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control SelectBox<T>(this Control parent, Func<T, string> render, Action<SLSelect<T>> action)
    {
        var select = new SLSelect<T>(render);
        action(select);
        parent.AddChild(select);
        return parent;
    }

    public static Control AddChildren(this Control parent, IEnumerable<Control> controls)
    {
        foreach (var control in controls)
            parent.AddChild(control);
        return parent;
    }

    public static Control WithVerticalExp(this Control parent)
    {
        parent.VerticalExpand = true;
        return parent;
    }

    public static Control WithHorizontalExp(this Control parent)
    {
        parent.HorizontalExpand = true;
        return parent;
    }

    public static Control WithVAlignment(this Control parent, Control.VAlignment alignment)
    {
        parent.VerticalAlignment = alignment;
        return parent;
    }

    public static Control WithMargin(this Control parent, Thickness thickness)
    {
        parent.Margin = thickness;
        return parent;
    }

    public static Control AddClass(this Control parent, string @class)
    {
        parent.AddStyleClass(@class);
        return parent;
    }

    public static Control Modulate(this Control parent, Color color)
    {
        parent.ModulateSelfOverride = color;
        return parent;
    }

    public static Control FixSize(this Control parent, Vector2 size)
    {
        parent.MinSize = size;
        parent.SetSize = size;
        parent.MaxSize = size;
        return parent;
    }
}