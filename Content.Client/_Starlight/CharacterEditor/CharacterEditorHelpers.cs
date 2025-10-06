// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;

namespace Content.Client._Starlight.CharacterEditor;

public static class CharacterEditorHelpers
{
    private const string PreviewModeLocPrefix = "character-editor-preview-mode-";
    public static string GetLocalizedPreviewMode(CharacterPreviewMode mode)
    {
        return Loc.GetString($"{PreviewModeLocPrefix}{mode.ToString().ToLower()}");
    }
}