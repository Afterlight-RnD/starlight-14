// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.ProfileEditor.UI;


public partial interface IProfileEditorFieldBuilder<TProfile>
{
    public void ProfileEditorFieldText(
        ProfileEditorField<TProfile> fieldControl,
        Func<TProfile, string> readData,
        Action<TProfile, string> writeData)
    {
        RegisterField(fieldControl, new ProfileEditorFieldText<TProfile>(readData, writeData));
    }
}

public sealed class ProfileEditorFieldText<TProfile>(
    Func<TProfile, string> readData,
    Action<TProfile, string> writeData)
    : LineEdit, IProfileEditorField<string, TProfile>
    where TProfile : class, IPersistentProfile<TProfile>
{
    private TProfile _data
    {
        get
        {
            if (_profile == null)
                throw new InvalidOperationException("Profile must be injected!");
            return _profile;
        }
    }
    private TProfile? _profile = null;
    public void InjectProfile(TProfile profile)
    {
        _profile = profile;
        profile.OnSync += FromProfile;
    }

    public void SetEditWidth(int width)
    {
        MinWidth = width;
    }


    public void FromProfile(TProfile profile)
    {
        SetText(readData.Invoke(profile), false);
    }
    public void ToProfile() => writeData.Invoke(_data, Text);
}