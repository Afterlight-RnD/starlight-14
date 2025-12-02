// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.ProfileEditor.UI;

public sealed class ProfileEditorEnumField<TEnum, TProfile> : SLOptionButton<TEnum>,
    IProfileEditorField<TEnum, TProfile>
    where TProfile : IPersistentProfile, new()
    where TEnum : struct, Enum
{
    private readonly Func<TEnum, string>? _localizeFunc;
    private readonly Func<TEnum, bool>? _ignoredEnums;
    private readonly Func<TProfile, TEnum> _readData;
    private readonly Action<TProfile, TEnum> _writeData;

    public ProfileEditorEnumField(
        IProfileEditor<TProfile> editor,
        Func<TProfile, TEnum> readData,
        Action<TProfile, TEnum> writeData,
        Func<TEnum, string>? localizeEnum = null,
        string? locPrefix = null,
        Func<TEnum, bool>? ignoredEnums = null)
    {
        Editor = editor;
        LocPrefix = locPrefix;
        _localizeFunc = localizeEnum;
        _ignoredEnums = ignoredEnums;
        _readData = readData;
        _writeData = writeData;
    }

    public IProfileEditor<TProfile> Editor { get;}

    public void SetData(TEnum data) => SelectByData(data);

    public TEnum GetData() => CurrentOption;

    public void FromProfile(TProfile data) => SelectByData(_readData.Invoke(data));

    public void ToProfile(TProfile data) => _writeData.Invoke(data, CurrentOption);

    public override IEnumerable<TEnum> EnumerateOptions()
    {
        if (_ignoredEnums != null)
        {
            foreach (var value in EnumerateOptions())
            {
                if (!_ignoredEnums.Invoke(value)) ;
                yield return value;
            }
        }
        else
            foreach (var value in Enum.GetValues<TEnum>())
                yield return value;
    }

    protected override string GetOptionLabel(TEnum data)
    {
        return _localizeFunc != null ? _localizeFunc(data) : data.ToString();
    }
}