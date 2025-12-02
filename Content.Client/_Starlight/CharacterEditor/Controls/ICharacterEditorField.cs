// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public interface ICharacterEditorField
{
    public bool Disabled { get; set; }
    public void Randomize();

    public static void SetData<TProfileData, TValue>(
        CharacterEditorMainMainControl? editorControl,
        TValue value, CharacterDataSetterDelegate<TProfileData, TValue> setter)
        where TProfileData : CharacterData, new()
    {
        if (EditorControl == null)
            return;
        var data = profile.GetData<TProfileData>();
        setter.Invoke(value, profile, data);
        data.Dirty();
        // UIEvents.RaiseEvent(new CharacterEditorProfileDirtiedUIEvent(profile));
    }

    public static void RandomizeField<TProfileData, TValue>(
        CharacterEditorMainMainControl? editorControl,
        Func<TValue> dataRandomizer, CharacterDataSetterDelegate<TProfileData, TValue> setter)
        where TProfileData : CharacterData, new()
    {
        var value = dataRandomizer.Invoke();
        SetData(profile, value, setter);
    }

    public static CharacterEditorDropdownEnumField<TData, TEnum> AddEnumField<TData,TEnum>(
        Control parent, CharacterDataSetterDelegate<TData, TEnum> setter, CharacterDataGetterDelegate<TData, TEnum> getter,
        string? locPrefix = null)
        where TData: CharacterData, new()
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
        CharacterDataSetterDelegate<TData, TPrototype> setter,
        CharacterDataGetterDelegate<TData, TPrototype> getter,
        Func<TPrototype, string>? localizedNameGetter = null)
        where TData : CharacterData, new()
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

    public static CharacterEditorTextField<TData> AddTextField<TData>(
        Control parent,
        CharacterDataSetterDelegate<TData, string> setter, CharacterDataGetterDelegate<TData, string> getter)
        where TData : CharacterData, new()
    {
        var editField =
            new CharacterEditorTextField<TData>
            {
                ProfileDataSetter = setter,
                ProfileDataGetter = getter,
            };
        parent.AddChild(editField);
        return editField;
    }
}