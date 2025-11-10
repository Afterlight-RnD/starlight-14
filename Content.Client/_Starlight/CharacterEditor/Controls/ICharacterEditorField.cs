// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public interface ICharacterEditorField
{
    public static void SetData<TProfileData, TValue>(TValue value,CharacterDataSetterDelegate<TProfileData, TValue> setter)
        where TProfileData : struct, ICharacterData
    {
        var profileEv = new EditCharacterProfileFieldUIRequest(null);
        UIEvents.RaiseRequest(ref profileEv);
        var dirty = profileEv.Profile?.EditData(value, setter);
        if (dirty == null || !dirty.Value)
            return;
        UIEvents.RaiseEvent(new CharacterEditorProfileDirtiedUIEvent(profileEv.Profile!));
    }
}