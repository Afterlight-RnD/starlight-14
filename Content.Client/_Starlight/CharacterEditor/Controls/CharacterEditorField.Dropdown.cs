// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Shared.Random;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public abstract class CharacterEditorDropdownField<TProfileData, TData> :
    SLOptionButton<TData>, ICharacterEditorField
    where TProfileData: struct, ICharacterData
{
    [Dependency] private readonly IRobustRandom _random = default!;

    public CharacterDataSetterDelegate<TProfileData, TData>? ProfileDataSetter;
    public CharacterDataGetterDelegate<TProfileData, TData>? ProfileDataGetter;

    protected override void ItemSelected(TData value)
    {
        if (ProfileDataSetter == null) return;
        ICharacterEditorField.SetData(value, ProfileDataSetter);
    }

    protected override void EnteredTree()
    {
        base.EnteredTree();
        SubscribeUIEvent<CharacterEditorProfileResetUIEvent>(HandleProfileReset);
        SubscribeUIEvent<CharacterEditorProfileDirtiedUIEvent>(HandleProfileDirtied);
    }

    private void HandleProfileDirtied(ref readonly CharacterEditorProfileDirtiedUIEvent args)
    {
        if (ProfileDataGetter == null) return;
        SelectByData(ProfileDataGetter.Invoke(args.Profile.GetData<TProfileData>()));
    }

    private void HandleProfileReset(ref readonly CharacterEditorProfileResetUIEvent args)
    {
        if (ProfileDataGetter == null) return;
        SelectByData(ProfileDataGetter.Invoke(args.Profile.GetData<TProfileData>()));
    }

    public void Randomize()
    {
        Select(_random.Next(OptionCount));
    }
}