using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.UI.Controls;

public abstract class SLDropdownBase: ContainerButton
{
    private SLDropdownOptions? _dropdown = null;

    SLDropdownBase()
    {
        ToggleMode = true;
        OnToggled += HandleToggled;

    }

    private SLDropdownOptions EnsureDropdown()
    {
        if (_dropdown != null)
            return _dropdown;
        _dropdown = new SLDropdownOptions();
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
        if (newChild is SLDropdownOption option)
        {
            dropDown.Orphan();
            dropDown.AddChild(option);
        }
    }
}