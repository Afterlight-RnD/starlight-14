// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.ProfileEditor.UI;
using Content.Shared._Starlight.CharacterProfiles;
using Content.Shared._Starlight.CharacterProfiles.Data;
using Content.Shared.Humanoid;

namespace Content.Client._Starlight.CharacterEditor.Editors.Appearance;

public sealed class CharacterEditorAppearanceStep : CharacterEditorStep
{
    public override string StepName => "Appearance";

    protected override void SetupStep(IProfileEditorPanelBuilder<CharacterProfile,CharacterEditorPanelLayout> panelBuilder)
    {
        panelBuilder.RegisterPanel<AppearancePanel>(CharacterEditorPanelLayout.Main,
            (panel, builder) =>
            {
                builder.RegisterFieldEnum(panel.CharacterBodyTypeField,
                    profile => profile.GetData<CharacterIdentityData>().BodyType,
                    (profile, sex) => profile.GetData<CharacterIdentityData>().BodyType = sex,
                    ignoredEnums:(sex => sex == Sex.Unsexed));
            });
    }
}