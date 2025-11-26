using Content.Client._Starlight.UI.Core;
using Content.Client.Stylesheets;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.CustomControls;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client._Starlight.UI;
[Virtual]
public class SLWindow : DefaultWindow, ISLControl
{
    private readonly IStylesheetManager _stylesheetManager = default!;
    internal SLWindow()
    {
        _stylesheetManager = IoCManager.Resolve<IStylesheetManager>();
        Stylesheet = _stylesheetManager.Starlight;
        CloseButton.Stylesheet = Stylesheet;
        CloseButton.AddStyleClass("CrossButtonRed");
    }
    public SLWindow Style(Func<IStylesheetManager, Stylesheet> func)
    {
        Stylesheet = func(_stylesheetManager);
        return this;
    }
    public SLWindow Grid(int columns, Action<SLGrid> action)
    {
        var grid = new SLGrid(columns)
        {
            Stylesheet = Stylesheet
        };
        action(grid);
        Contents.AddChild(grid);
        return this;
    }
    public SLWindow Scroll(Action<SLScroll> action)
    {
        var scroll = new SLScroll()
        {
            Stylesheet = Stylesheet
        };
        action(scroll);
        Contents.AddChild(scroll);
        return this;
    }
    public SLWindow SelectBox<T>(Func<T, string> render, Action<SLSelect<T>> action)
    {
        var select = new SLSelect<T>(render)
        {
            Stylesheet = Stylesheet
        };
        action(select);
        Contents.AddChild(select);
        return this;
    }
    public SLWindow Box(LayoutOrientation orientation, Action<SLBox> action)
    {
        var select = new SLBox(orientation)
        {
            Stylesheet = Stylesheet
        };
        action(select);
        Contents.AddChild(select);
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