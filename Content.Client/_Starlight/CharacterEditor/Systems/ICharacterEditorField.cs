// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public interface ICharacterEditorField
{
    protected CharacterEditorSystem EditorSystem { get; set; }

    protected void SetupEditorField()
    {
        var systemRequest = new InjectCharacterEditorSystemUIRequest();
        UIEvents.RaiseRequest(ref systemRequest);
        EditorSystem = systemRequest.EditorSystem;
    }

    protected void SetData<TData>(CharacterDataSetterDelegate<TData> setter) where TData : struct, ICharacterData
    {
        EditorSystem.SetLiveData(setter);
    }
}