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
public record struct ApplyCharacterProfileChangesUIEvent(int Slot);
public record struct CharacterEditorPreviewChangedUIEvent(Entity<SpriteComponent> PreviewEntity);

public record struct CharacterEditorPreviewNameChangedUIEvent(string NewName);

public record struct ChangeCharacterEditorPreviewModeUIEvent(CharacterPreviewMode NewMode);

public record struct SelectCharacterProfileUIEvent(int Slot);
public record struct DeleteCharacterProfileUIEvent(int Slot);
public record struct DiscardCharacterProfileChangesUIEvent();
public record struct SaveCharacterProfileChangesUIEvent();
public record struct SetEnableCharacterProfileUIEvent(int Slot, bool ActiveState);
//== UI Requests ==
public record struct EditCharacterProfileFieldUIRequest(CharacterProfile? Profile)
{
    public bool HasProfile => Profile != null;
}