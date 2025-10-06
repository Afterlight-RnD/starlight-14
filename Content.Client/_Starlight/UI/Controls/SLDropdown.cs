using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Controls;

[Virtual]
public class SLDropdown: ContainerButton
{
    private SLDropdownPopout? _dropdown = null;
    public Vector2 DropdownOffset { get; set; } = new(0, 5);

    public SLDropdown()
    {
        OnPressed += HandlePressed;
    }

    private void HandlePressed(ButtonEventArgs args)
    {
        var dropdown = EnsureDropdown();
        if (dropdown.Visible)
            return;
        dropdown.Open(GetPopupPos(dropdown));
    }

    private UIBox2 GetPopupPos(SLDropdownPopout dropdown)
    {
        var globalLeft = GlobalPosition.X;
        var globalBot = GlobalPosition.Y + Height;
        return UIBox2.FromDimensions(
            new Vector2(globalLeft, globalBot),
            new Vector2(Width + DropdownOffset.X, DropdownOffset.Y));
    }

    protected override void EnteredTree()
    {
        if (_dropdown == null)
            return;
        UserInterfaceManager.ModalRoot.AddChild(_dropdown);
    }

    protected override void ExitedTree()
    {
        _dropdown?.Orphan();
    }

    private SLDropdownPopout EnsureDropdown()
    {
        if (_dropdown != null)
            return _dropdown;
        _dropdown = new SLDropdownPopout();
        return _dropdown;
    }

    public void AddDropdownOption<T>(T option) where T: BaseButton, IDropdownControlOption
    {
        option.Orphan();
        var dropDown = EnsureDropdown();
        option.OnPressed += HandleOptionPressed;
        dropDown.Contents.AddChild(option);
    }

    private void HandleOptionPressed(ButtonEventArgs obj)
    {
        _dropdown?.Close();
    }

    protected override void ChildAdded(Control newChild)
    {
        var dropDown = EnsureDropdown();
        if (newChild is not IDropdownControlOption) return;
        newChild.Orphan();
        dropDown.Contents.AddChild(newChild);
    }
}