// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT


using Content.Client._Starlight.UI;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.ProfileEditor.UI;

[Virtual]
public class ProfileEditorStepButton : SLContainerButton
{
    public int Step { get; private set; }

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

    public void SetFromStep(int step, string stepName, string? stepDesc, Texture? stepTexture)
    {
        Step = step;
        Label = LocalizeLabel(stepName);
        Description = LocalizeDescription(stepDesc);
        Icon = stepTexture;
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
}