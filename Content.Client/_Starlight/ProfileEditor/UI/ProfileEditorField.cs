// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.ProfileEditor.UI;

public sealed class ProfileEditorField : SLBox
{
    public string? Label { get => _label.Text; set => _label.Text = value; }

    private readonly Label _label = new();
    private IProfileEditorField? _field = null;
    protected ProfileEditorField()
    {
        HorizontalExpand = true;
        VerticalExpand = true;
        Margin = new Thickness(10);
        SeparationOverride = 5;
        AddChild(_label);
    }
    public void InitializeAsFieldType<TFieldType>(TFieldType field)
    where TFieldType: Control,IProfileEditorField
    {
        if (_field != null)
            throw new InvalidOperationException($"Field:{Name} is already registered as type:{field.GetType()}");
        _field = field;
        AddChild(field);
    }
}