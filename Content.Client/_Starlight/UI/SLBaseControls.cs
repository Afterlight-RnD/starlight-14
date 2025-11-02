// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Numerics;
using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.UI;

[Virtual]
public class SLLayout : LayoutContainer
{
}

[Virtual]
public class SLGrid : GridContainer
{
    internal SLGrid(int columns)
    {
        Columns = columns;
        HorizontalAlignment = HAlignment.Stretch;
    }
}

[Virtual]
public class SLSelect<T> : OptionButton
{
    private List<T> _items = [];
    private readonly Func<T, string> _render = null!;
    private event Action<T> _onItemSelected = delegate { };
    internal SLSelect(Func<T, string> render)
    {
        _render = render;
        _onItemSelected += (item) => _items.Remove(item);
    }

    public List<T> Items
    {
        get => _items;
        set
        {
            _items = value;
            StateHasChanged();
        }
    }
    public SLSelect<T> SetItems(List<T> items)
    {
        Items = items;
        return this;
    }
    public SLSelect<T> StateHasChanged()
    {
        var i = -1;
        foreach (var item in _items)
            AddItem(_render(item), ++i);
        return this;
    }
    public SLSelect<T> Bind(Action<T> handler)
    {
        _onItemSelected += handler;
        return this;
    }
}

public sealed class SLStripe : StripeBack
{
}

[Virtual]
public class SLTextureRect : TextureRect
{
}

[Virtual]
public class SLLayeredTextureRect : LayeredTextureRect
{
}

[Virtual]
public class SLScroll : ScrollContainer
{
}

[Virtual]
public class SLPanel : PanelContainer
{
}
[Virtual]
public class SLButton : Button
{
}

[Virtual]
public class SLButtonWithShader : Button
{
    public ShaderInstance? ShaderInstance { get; private set; }
    public SLButtonWithShader WithShader(ProtoId<ShaderPrototype> shader)
    {
        if(!IoCManager.Resolve<IPrototypeManager>().TryIndex(shader, out var shaderProto))
            return this;
        ShaderInstance = shaderProto.Instance();
        return this;
    }
    public SLButtonWithShader WithShader(ShaderInstance shader)
    {
        ShaderInstance = shader;
        return this;
    }
    protected override void Draw(IRenderHandle renderHandle)
    {
        renderHandle.DrawingHandleScreen.UseShader(ShaderInstance);
        base.Draw(renderHandle);
    }
}

[Virtual]
public class SLTextureButton : TextureButton
{
}

[Virtual]
public class SLLabel : Label
{
    public SLLabel WithText(string text)
    {
        Text = text;
        return this;
    }
    public SLLabel WithFont(string path, int size)
    {
        var font = new VectorFont(IoCManager.Resolve<IResourceCache>().GetResource<FontResource>(path), size);
        FontOverride = font;
        return this;
    }

}

[Virtual]
public class SLRichTextLabel : RichTextLabel
{
    public RichTextLabel WithText(string text)
    {
        Text = text;
        return this;
    }

}


public sealed class Subscription(Action onDisposed) : IDisposable
{
    public void Dispose() => onDisposed?.Invoke();
}