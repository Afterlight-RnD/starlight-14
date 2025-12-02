// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.ProfileEditor;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem : ProfileEditorSystem<CharacterEditor,CharacterEditorMainMainControl>
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;

}