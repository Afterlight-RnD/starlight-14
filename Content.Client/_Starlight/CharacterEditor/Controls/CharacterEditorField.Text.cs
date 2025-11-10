// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.UI;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Controls;

[Virtual]
public class CharacterEditorLineEditField<TProfileData> : SLLineEdit, ICharacterEditorField
where TProfileData: struct, ICharacterData
{
    public bool Disabled { get => !Editable; set => Editable = !value; }

    public CharacterDataSetterDelegate<TProfileData, string>? ProfileDataSetter;
    public CharacterDataGetterDelegate<TProfileData, string>? ProfileDataGetter;
    public Func<string>? DataRandomizer;


    protected override void EnteredTree()
    {
        SubscribeUIEvent<CharacterEditorProfileResetUIEvent>(HandleProfileReset);
        SubscribeUIEvent<CharacterEditorProfileDirtiedUIEvent>(HandleProfileDirtied);
    }

    private void HandleProfileDirtied(ref readonly CharacterEditorProfileDirtiedUIEvent args)
    {
        if (ProfileDataGetter == null) return;
        SetText(ProfileDataGetter.Invoke(args.Profile.GetData<TProfileData>()));
    }

    private void HandleProfileReset(ref readonly CharacterEditorProfileResetUIEvent args)
    {
        if (ProfileDataGetter == null) return;
        SetText(ProfileDataGetter.Invoke(args.Profile.GetData<TProfileData>()));
    }

    public void Randomize()
    {
        if (DataRandomizer == null || ProfileDataSetter == null)
            return;
        ICharacterEditorField.RandomizeField(DataRandomizer, ProfileDataSetter);
    }

    public CharacterEditorLineEditField()
    {
        OnTextEntered += HandleTextEntered;
    }

    private void HandleTextEntered(LineEditEventArgs obj)
    {
        if (ProfileDataSetter == null) return;
        ICharacterEditorField.SetData(obj.Text, ProfileDataSetter);
    }

}