// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public interface ICharacterEditorField
{
    public bool Disabled { get; set; }
    public void Randomize();

    public static void SetData<TProfileData, TValue>(TValue value,CharacterDataSetterDelegate<TProfileData, TValue> setter)
        where TProfileData : struct, ICharacterData
    {
        var profileEv = new EditCharacterProfileFieldUIRequest(null);
        UIEvents.RaiseRequest(ref profileEv);
        if (profileEv.Profile == null)
            return;
        profileEv.Profile?.EditData(value, setter);
        UIEvents.RaiseEvent(new CharacterEditorProfileDirtiedUIEvent(profileEv.Profile!));
    }

    public static void RandomizeField<TProfileData, TValue>(Func<TValue> dataRandomizer,
        CharacterDataSetterDelegate<TProfileData, TValue> setter)
        where TProfileData : struct, ICharacterData
    {
        var profileEv = new EditCharacterProfileFieldUIRequest(null);
        UIEvents.RaiseRequest(ref profileEv);
        if (profileEv.Profile == null)
            return;
        var value = dataRandomizer.Invoke();
        profileEv.Profile?.EditData(value, setter);
        UIEvents.RaiseEvent(new CharacterEditorProfileDirtiedUIEvent(profileEv.Profile!));
    }

    public static CharacterEditorDropdownEnumField<TData, TEnum> AddEnumField<TData,TEnum>(
        Control parent,
        CharacterDataGetterDelegate<TData, TEnum> getter,
        CharacterDataSetterDelegate<TData, TEnum> setter,
        string? locPrefix = null)
        where TData: struct, ICharacterData
        where TEnum : struct, Enum
    {
        var dropdown = new CharacterEditorDropdownEnumField<TData, TEnum>
        {
            LocPrefix = locPrefix,
            ProfileDataSetter = setter,
            ProfileDataGetter = getter
        };
        parent.AddChild(dropdown);
        return dropdown;
    }

    public static CharacterEditorDropdownPrototypeField<TData, TPrototype> AddPrototypeField<TData, TPrototype>(
        Control parent,
        CharacterDataGetterDelegate<TData, TPrototype> getter,
        CharacterDataSetterDelegate<TData, TPrototype> setter,
        Func<TPrototype, string>? localizedNameGetter = null)
        where TData : struct, ICharacterData
        where TPrototype : class, IPrototype
    {
        var dropdown =
            new CharacterEditorDropdownPrototypeField<TData, TPrototype>
            {
                ProfileDataSetter = setter,
                ProfileDataGetter = getter,
                LocalizedNameGetter = localizedNameGetter
            };
        parent.AddChild(dropdown);
        return dropdown;
    }

    public static CharacterEditorLineEditField<TData> AddTextField<TData>(
        Control parent,
        CharacterDataGetterDelegate<TData, string> getter,
        CharacterDataSetterDelegate<TData, string> setter)
        where TData : struct, ICharacterData
    {
        var editField =
            new CharacterEditorLineEditField<TData>
            {
                ProfileDataSetter = setter,
                ProfileDataGetter = getter,
            };
        parent.AddChild(editField);
        return editField;
    }
}