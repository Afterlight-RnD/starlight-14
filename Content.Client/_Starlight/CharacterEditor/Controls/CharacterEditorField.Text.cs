// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.UI;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Controls;

[Virtual]
public class CharacterEditorTextField<TProfileData> : SLLineEdit, ICharacterEditorField
where TProfileData: CharacterData, new()
{
    private CharacterProfile? _profile = null;

    public bool Disabled { get => !Editable; set => Editable = !value; }

    public CharacterDataSetterDelegate<TProfileData, string>? ProfileDataSetter;
    public CharacterDataGetterDelegate<TProfileData, string>? ProfileDataGetter;
    public Func<string>? DataRandomizer;


    protected override void EnteredTree()
    {
        base.EnteredTree();
        SubscribeUIEvent<CharacterEditorProfileResetUIEvent>(HandleProfileReset);
        SubscribeUIEvent<CharacterEditorProfileDirtiedUIEvent>(HandleProfileDirtied);
    }

    private void HandleProfileDirtied(ref readonly CharacterEditorProfileDirtiedUIEvent args)
    {
        _profile = args.Profile;
        if (ProfileDataGetter == null) return;
        SetText(ProfileDataGetter.Invoke(args.Profile.GetData<TProfileData>()));
    }

    private void HandleProfileReset(ref readonly CharacterEditorProfileResetUIEvent args)
    {
        _profile = args.Profile;
        if (ProfileDataGetter == null) return;
        SetText(ProfileDataGetter.Invoke(args.Profile.GetData<TProfileData>()));
    }

    public void Randomize()
    {
        if (_profile == null || DataRandomizer == null || ProfileDataSetter == null)
            return;
        ICharacterEditorField.RandomizeField(_profile, DataRandomizer, ProfileDataSetter);
    }

    public CharacterEditorTextField()
    {
        OnTextEntered += HandleTextEntered;
    }

    protected virtual void HandleTextEntered(LineEditEventArgs obj)
    {
        if (ProfileDataSetter == null) return;
        ICharacterEditorField.SetData(_profile, obj.Text, ProfileDataSetter);
    }

}