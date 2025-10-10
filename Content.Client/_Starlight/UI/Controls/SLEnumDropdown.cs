// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Label = Robust.Client.UserInterface.Controls.Label;

namespace Content.Client._Starlight.UI.Controls;


public abstract class SLValueDropdown<T> : SLDropdown where T: notnull
{
    private T _selectedValue;

    public T SelectedValue
    {
        get => _selectedValue;
        private set
        {
            _selectedValue = value;
            Label.Text = GetValueString(value);
        }
    }

    public abstract string LocPrefix { get; }

    public abstract string GetLocStringForValue(T value);

    public abstract IEnumerable<T> IterateValues();

    protected Label Label;

    public SLValueDropdown(T initialValue)
    {
        Label = new Label();
        AddChild(Label);
        _selectedValue = initialValue;
    }

    protected override void EnteredTree()
    {
        Label.Text = GetValueString(SelectedValue);
        base.EnteredTree();
    }

    public string GetValueString(T value)
    {
        return Loc.GetString($"{LocPrefix}-{GetLocStringForValue(value)}");
    }

    protected override void DropdownCreated(SLDropdownPopout popout)
    {
        foreach (var value in IterateValues())
        {
            var newOption = new Option(value)
            {
                Text = GetValueString(value)
            };
            newOption.OnPressed += args =>
            {
                var selector = (Option)args.Button;
                if (SelectedValue.Equals(selector.Value))
                {
                    popout.Close();
                    return;
                }
                UserInterfaceManager.RaiseUIEvent(this, new ValueChanged(SelectedValue, selector.Value));
                SelectedValue = selector.Value;
                popout.Close();
            };
            AddChild(newOption);
        }
    }
    public record struct ValueChanged(T OldValue,T NewValue);

    [Virtual]
    public class Option: SLButton, IDropdownControlOption
    {
        public T Value { get; private init; }

        public Option(T value)
        {
            Value = value;
        }
    }
}
