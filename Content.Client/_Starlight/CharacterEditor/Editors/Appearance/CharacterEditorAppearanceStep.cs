// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Editors.Appearance;

public sealed class CharacterEditorAppearanceStep : CharacterEditorStep
{
    public override string StepName => "Appearance";

    protected override void SetupStep(CharacterEditor editor)
    {
        RegisterPanel<AppearancePanel>(CharacterEditorPanelLayout.Main, OnLoadProfile,OnSaveProfile);
    }

    private void OnLoadProfile(AppearancePanel panel, CharacterProfile profile)
    {

    }

    private void OnSaveProfile(AppearancePanel panel, CharacterProfile profile)
    {

    }
}