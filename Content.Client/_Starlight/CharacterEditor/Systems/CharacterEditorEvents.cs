// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterEditor.Systems;


//== UI Events ==
public record struct EnterCharacterEditorUIEvent();
public record struct ExitCharacterEditorUIEvent(bool SaveChanges);
public record struct CharacterEditorCreateProfileUIEvent();
public record struct CharacterEditorProfileDirtiedUIEvent(CharacterProfile Profile);

public record struct CharacterEditorProfileAppliedUIEvent(CharacterProfile Profile);

public record struct CharacterEditorPreviewChangedUIEvent(Entity<SpriteComponent> PreviewEntity, string Name);

public record struct CharacterEditorPreviewNameChangedUIEvent(string NewName);

public record struct ChangeCharacterEditorPreviewMode(CharacterPreviewMode NewMode);

public record struct SelectCharacterProfileUIEvent(int Slot);
public record struct DeleteCharacterProfileUIEvent(int Slot);
public record struct SetEnableCharacterProfileUIEvent(int Slot, bool ActiveState);
//== UI Requests ==