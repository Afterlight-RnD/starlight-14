// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Controls;
using Content.Client._Starlight.ProfileEditor;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor;


public sealed class CharacterEditor : ProfileEditor<CharacterEditor, CharacterProfile, CharacterEditorMainControl,
    CharacterEditorBasePanel, CharacterEditorStepButton,CharacterEditorStep, CharacterEditorPanelPosition>
{
}