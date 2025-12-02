// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.ProfileEditor;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem : ProfileEditorSystem<CharacterProfile,CharacterEditor,CharacterEditorMainControl, CharacterEditorStepButton>
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;
}

public abstract class CharacterEditorStep : ProfileEditorStep<CharacterProfile, CharacterEditor,
    CharacterEditorMainControl, CharacterEditorPanelLayout>
{

}