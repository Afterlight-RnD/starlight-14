// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public interface ICharacterEditorField
{
    protected void SetData<TData>(CharacterDataSetterDelegate<TData> setter)
        where TData : struct, ICharacterData
    {
        var profileEv = new EditCharacterProfileFieldUIRequest(null);
        UIEvents.RaiseRequest(ref profileEv);
        var dirty = profileEv.Profile?.EditData(setter);
        if (dirty == null || !dirty.Value)
            return;
        UIEvents.RaiseEvent(new CharacterEditorProfileDirtiedUIEvent(profileEv.Profile!));
    }
}