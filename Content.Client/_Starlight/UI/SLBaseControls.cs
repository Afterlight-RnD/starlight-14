// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.UI.Core;
using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.UI;

[Virtual]
public class SLLayout : LayoutContainer, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLGrid : GridContainer, ISLControl
{
    internal SLGrid(int columns)
    {
        Columns = columns;
        HorizontalAlignment = HAlignment.Stretch;
    }

    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLSelect<T> : OptionButton, ISLControl
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

    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

public sealed class SLStripe : StripeBack, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLTextureRect : TextureRect, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLLayeredTextureRect : LayeredTextureRect, ISLControl
{

    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLScroll : ScrollContainer, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLPanel : PanelContainer, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLButton : Button, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLContainerButton : ContainerButton, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents
    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();
    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }
        _uiEventHandles.Clear();
    }
    #endregion

}

[Virtual]
public class SLButtonWithShader : Button, ISLControl
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
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents
    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();
    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }
        _uiEventHandles.Clear();
    }
    #endregion
}

[Virtual]
public class SLTextureButton : TextureButton, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}

[Virtual]
public class SLLabel : Label, ISLControl
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

    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents
    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();
    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }
        _uiEventHandles.Clear();
    }
    #endregion
}

[Virtual]
public class SLLineEdit : LineEdit, ISLControl
{
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents
    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();
    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T: struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }
        _uiEventHandles.Clear();
    }
    #endregion
}

[Virtual]
public class SLRichTextLabel : RichTextLabel, ISLControl
{
    public RichTextLabel WithText(string text)
    {
        Text = text;
        return this;
    }
    [MustCallBase]
    protected override void EnteredTree()
    {
        RaiseUIEvent(new ControlEnteredTreeUIEvent());
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        RaiseUIEvent(new ControlExitedTreeUIEvent());
        UnsubscribeAllUIEvents();
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}


public sealed class SLButtonSubscription(Action onDisposed) : IDisposable
{
    public void Dispose() => onDisposed?.Invoke();
}