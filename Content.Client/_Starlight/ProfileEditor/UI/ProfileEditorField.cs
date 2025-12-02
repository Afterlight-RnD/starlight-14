// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.ProfileEditor.UI;

public abstract class ProfileEditorField<TProfile> : SLBox, IProfileEditorListenerControl<TProfile>
where TProfile: class, IPersistentProfile
{
    public string? Label { get => _label.Text; set => _label.Text = value; }
    public virtual int EditingWidth { get; set; }

    private readonly Label _label = new();
    private IProfileEditorField? _field = null;
    private TProfile? _profile = null;
    public void InjectProfile(TProfile profile) => _profile = profile;
    protected ProfileEditorField()
    {
        Access = AccessLevel.Public;
        HorizontalExpand = true;
        VerticalExpand = true;
        Margin = new Thickness(10);
        SeparationOverride = 5;
        AddChild(_label);
    }
    public void InitializeAsFieldType<TFieldType>(TFieldType field)
    where TFieldType: Control, IProfileEditorField<TProfile>
    {
        if (_field != null)
            throw new InvalidOperationException($"Field:{Name} is already registered as type:{field.GetType()}");
        _field = field;
        field.SetEditWidth(EditingWidth);
        if (_profile == null)
            throw new InvalidOperationException("Profile Must Be Injected Before Field Initialization");
        field.InjectProfile(_profile);
        AddChild(field);
    }
}