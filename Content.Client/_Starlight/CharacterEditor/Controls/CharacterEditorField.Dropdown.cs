// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Shared.Random;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public abstract class CharacterEditorDropdownField<TProfileData, TData> :
    SLOptionButton<TData>, ICharacterEditorField
    where TProfileData: CharacterData, new()
{
    [Dependency] private readonly IRobustRandom _random = default!;

    private CharacterProfile? _profile = null;

    public CharacterEditorField? ParentFieldControl
    {
        get
        {
            if (Parent is  CharacterEditorField field)
                return field;
            return null;
        }
    }


    public CharacterDataSetterDelegate<TProfileData, TData>? ProfileDataSetter;
    public CharacterDataGetterDelegate<TProfileData, TData>? ProfileDataGetter;

    protected override void ItemSelected(TData value)
    {
        if (ProfileDataSetter == null) return;
        ICharacterEditorField.SetData(_profile, value, ProfileDataSetter);
    }

    protected override void EnteredTree()
    {
        base.EnteredTree();
        if (ParentFieldControl == null)
            return;
    }

    // private void HandleProfileDirtied(ref readonly CharacterEditorProfileDirtiedUIEvent args)
    // {
    //     _profile = args.Profile;
    //     if (ProfileDataGetter == null) return;
    //     SelectByData(ProfileDataGetter.Invoke(args.Profile.GetData<TProfileData>()));
    // }
    //
    // private void HandleProfileReset(ref readonly CharacterEditorProfileResetUIEvent args)
    // {
    //     _profile = args.Profile;
    //     if (ProfileDataGetter == null) return;
    //     SelectByData(ProfileDataGetter.Invoke(args.Profile.GetData<TProfileData>()));
    // }

    public void Randomize()
    {
        Select(_random.Next(OptionCount));
    }
}