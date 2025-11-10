// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterEditor.Systems;
using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.CharacterEditor.Controls;

public abstract class CharacterEditorDropdownField<TProfileData, TData> : SLOptionButton<TData>, ICharacterEditorField
    where TProfileData: struct, ICharacterData
{
    public abstract TData Data { get; protected set; }

    public event CharacterDataSetterDelegate<TProfileData, TData>? SetProfileData;
    public event CharacterDataGetterDelegate<TProfileData, TData>? GetProfileData;

    protected override void ItemSelected(TData value)
    {
        if (SetProfileData == null) return;
        ICharacterEditorField.SetData(value, SetProfileData);
        Data = value;
    }

    private void ResetFromProfile(TProfileData profileData)
    {
        if (GetProfileData != null)
            Data = GetProfileData.Invoke(profileData);
    }

    protected override void EnteredTree()
    {
        base.EnteredTree();
        SubscribeUIEvent<CharacterEditorProfileResetUIEvent>(HandleProfileReset);
    }

    private void HandleProfileReset(ref readonly CharacterEditorProfileResetUIEvent args)
    {
        ResetFromProfile(args.Profile.GetData<TProfileData>());
    }
}