// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.ProfileEditor.UI;

public sealed class ProfileEditorOptionField<TData, TProfile> : SLOptionButton<TData>, IProfileEditorField<TData,TProfile>
    where TProfile : IPersistentProfile, new()
{
    public IProfileEditor<TProfile> Editor { get; }

    private readonly Func<IEnumerable<TData>> _getOptions;
    private readonly Func<TData,string> _getLabel;
    private readonly Func<TProfile, TData> _readData;
    private readonly Action<TProfile, TData> _writeData;

    public ProfileEditorOptionField(
        IProfileEditor<TProfile> editor,
        Func<TProfile, TData> readData,
        Action<TProfile, TData> writeData,
        Func<IEnumerable<TData>> getOptions,
        Func<TData,string> getOptionLabel,
        string? locPrefix = null)
    {
        Editor = editor;
        _readData = readData;
        _writeData = writeData;
        _getOptions = getOptions;
        _getLabel = getOptionLabel;
        LocPrefix = locPrefix;
    }


    public void FromProfile(TProfile profile) => SelectByData(_readData.Invoke(profile));

    public void ToProfile(TProfile profile) => _writeData.Invoke(profile, CurrentOption);
    public override IEnumerable<TData> EnumerateOptions() => _getOptions.Invoke();
    protected override string GetOptionLabel(TData data) => _getLabel.Invoke(data);
}