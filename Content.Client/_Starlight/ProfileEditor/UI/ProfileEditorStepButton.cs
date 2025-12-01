// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT


using Content.Client._Starlight.UI;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.ProfileEditor.UI;

public abstract class ProfileEditorStepButton<TStepEnum> : SLContainerButton
where TStepEnum: struct, Enum, IConvertible
{
    public TStepEnum Step { get; private set; }
    public virtual string? StepLocPrefix => null;
    public virtual string? StepDescriptionLocString => null;

    public Texture? Icon { get => _icon.Texture; set => _icon.Texture = value; }
    public string IconPath { set => _icon.TexturePath = value; }

    public string? Label { get => _mainText.Text; set => _mainText.Text = value; }
    public string? Description { get => _mainText.Text; set => _mainText.Text = value; }

    private TextureRect _icon = new();

    private Label _mainText = new ();

    private Label _descText = new ();

    public ProfileEditorStepButton()
    {
        var mainBox = new BoxContainer { Orientation = BoxContainer.LayoutOrientation.Horizontal };
        mainBox.AddChild(_icon);
        var secBox = new BoxContainer() { Orientation = BoxContainer.LayoutOrientation.Vertical };
        mainBox.AddChild(secBox);
        secBox.AddChild(_mainText);
        secBox.AddChild(_descText);
        AddChild(mainBox);
    }

    public void SetFromStep(TStepEnum step, IResourceCache resCache)
    {
        Step = step;
        Icon = GetIcon(step, resCache);
        Label = LocalizeLabel(GetLabel(step));
        Description = LocalizeDescription(GetDescription(step));
    }

    protected virtual Texture? GetIcon(TStepEnum step, IResourceCache resCache)
    {
        return null;
    }

    private string? LocalizeDescription(string? description)
    {
        return StepDescriptionLocString == null
            ? description
            : Loc.GetString($"{StepDescriptionLocString}-{description}");
    }

    private string LocalizeLabel(string label)
    {
        return StepLocPrefix == null ? label : Loc.GetString($"{StepLocPrefix}-{label}");
    }

    protected virtual string? GetDescription(TStepEnum step)
    {
        return null;
    }

    protected virtual string GetLabel(TStepEnum step)
    {
        return step.ToString().ToLower();
    }


}