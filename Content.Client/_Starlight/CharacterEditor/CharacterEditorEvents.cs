// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor;


// == Events ==

//== UI Events ==
public record struct ExitEditorUIEvent(CharacterEditorControl EditorControl);
public record struct EditorIsDirtyUIEvent(CharacterEditorControl EditorControl);
public record struct EditorChangesAppliedUIEvent(CharacterEditorControl EditorControl);
public record struct EditorChangesDiscardedUIEvent(CharacterEditorControl EditorControl);

public record struct ProfileSlotEnabledChangeUIEvent(int ProfileSlot, bool Enable);