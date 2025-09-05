using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Controls;

[Virtual]
public class SLDropdown: ContainerButton
{
    private SLDropdownPopout? _dropdown = null;

    public SLDropdown()
    {
        ToggleMode = true;
        OnToggled += HandleToggled;

    }

    private SLDropdownPopout EnsureDropdown()
    {
        if (_dropdown != null)
            return _dropdown;
        _dropdown = new SLDropdownPopout();
        _dropdown.Visible = false;
        return _dropdown;
    }

    private void HandleToggled(ButtonToggledEventArgs args)
    {
        var dropdown = EnsureDropdown();
        if (args.Pressed)
        {
            dropdown.Visible = true;
            return;
        }
        dropdown.Visible = false;
    }

    public void AddDropdownOption(SLDropdownOption option)
    {
        option.Orphan();
        var dropDown = EnsureDropdown();
        dropDown.AddChild(option);
    }

    protected override void ChildAdded(Control newChild)
    {
        var dropDown = EnsureDropdown();
        if (newChild is not IDropdownControlOption) return;
        newChild.Orphan();
        dropDown.AddChild(newChild);
    }
}