using System.Numerics;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Shared.Prototypes;
using static Robust.Client.UserInterface.Control;
using static Robust.Client.UserInterface.Controls.BaseButton;
using static Robust.Client.UserInterface.Controls.BoxContainer;

namespace Content.Client._Starlight.UI;
internal sealed class SLWindow : DefaultWindow
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
}