// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.ProfileEditor.UI;

public partial interface IProfileEditorFieldBuilder<TProfile>
{
    public void ProfileEditorFieldOption<TData>(ProfileEditorField<TProfile> fieldControl,
        Func<TProfile, TData> readData,
        Action<TProfile, TData> writeData,
        Func<IEnumerable<TData>> getOptions,
        Func<TData, string> getOptionLabel,
        string? locPrefix = null)
    {
        RegisterField(fieldControl,
            new ProfileEditorFieldOption<TData, TProfile>(readData, writeData, getOptions, getOptionLabel, locPrefix));
    }
}

public sealed class ProfileEditorFieldOption<TData, TProfile> : SLOptionButton<TData>, IProfileEditorField<TData,TProfile>
    where TProfile : class, IPersistentProfile<TProfile>
{
    private readonly Func<IEnumerable<TData>> _getOptions;
    private readonly Func<TData,string> _getLabel;
    private readonly Func<TProfile, TData> _readData;
    private readonly Action<TProfile, TData> _writeData;

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

    public ProfileEditorFieldOption(
        Func<TProfile, TData> readData,
        Action<TProfile, TData> writeData,
        Func<IEnumerable<TData>> getOptions,
        Func<TData,string> getOptionLabel,
        string? locPrefix = null)
    {
        _readData = readData;
        _writeData = writeData;
        _getOptions = getOptions;
        _getLabel = getOptionLabel;
        LocPrefix = locPrefix;
    }


    public void FromProfile(TProfile profile) => SelectByData(_readData.Invoke(profile));

    public void ToProfile() => _writeData.Invoke(_data, CurrentOption);
    public override IEnumerable<TData> EnumerateOptions() => _getOptions.Invoke();
    protected override string GetOptionLabel(TData data) => _getLabel.Invoke(data);
}