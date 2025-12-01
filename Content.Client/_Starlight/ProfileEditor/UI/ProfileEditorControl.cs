// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;


public interface IProfileEditorControl<TProfile> : ISLControl
    where TProfile : class, IPersistentProfile, new()
{
    public bool StepControlsInjected { get; set; }
    public ProfileEditor<TProfile> Editor { get; }

    public void AddStepButton(ProfileEditorStepButton button);
}
public abstract class ProfileEditorControl<TProfile, TPanelEnum, TBasePanelControl> : SLControl, IProfileEditorControl<TProfile>
    where TProfile : class, IPersistentProfile, new()
    where TPanelEnum: struct, Enum, IConvertible
    where TBasePanelControl: SLControl
{
    public abstract Control StepSelectorRoot { get; }
    public bool StepControlsInjected { get; set; }
    public ProfileEditor<TProfile> Editor { get; set; }= new();
    void IProfileEditorControl<TProfile>.AddStepButton(ProfileEditorStepButton button)
    {
        StepSelectorRoot.AddChild(button);
    }

    public abstract bool InjectPanel(TPanelEnum panelEnum, TBasePanelControl panel);
}