// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Controls;

[Virtual]
public class CharacterEditorDropdownEnumField<TProfileData, TEnum>
    : CharacterEditorDropdownField<TProfileData, TEnum>
    where TProfileData : struct, ICharacterData
    where TEnum: struct, Enum
{
    public override IEnumerable<TEnum> EnumerateOptions()
    {
        foreach (var value in Enum.GetValues<TEnum>())
            yield return value;
    }

    protected override string GetOptionLabel(TEnum data)
    {
        return data.ToString();
    }
}