// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor;


// == Events ==

//== UI Events ==
public record struct EnterEditorUIEvent;
public record struct ExitEditorUIEvent;
public record struct EditorIsDirtyUIEvent;
public record struct EditorChangesAppliedUIEvent;
public record struct EditorChangesDiscardedUIEvent;

public record struct ProfileSlotEnabledChangeUIEvent(int ProfileSlot, bool Enable);