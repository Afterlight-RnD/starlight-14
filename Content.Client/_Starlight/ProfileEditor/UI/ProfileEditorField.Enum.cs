// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.ProfileEditor.UI;


// public partial interface IProfileEditorFieldBuilder<TProfile>
// {
//     public void RegisterFieldEnum<TEnum>(ProfileEditorField<TProfile> fieldControl,
//         Func<TProfile, TEnum> readData,
//         Action<TProfile, TEnum> writeData,
//         Func<TEnum, string>? localizeEnum = null,
//         string? locPrefix = null,
//         Func<TEnum, bool>? ignoredEnums = null)
//         where TEnum : struct, Enum
//     {
//         RegisterField(fieldControl,
//             new ProfileEditorFieldEnum<TEnum, TProfile>(readData, writeData, localizeEnum, locPrefix, ignoredEnums));
//     }
// }
//
// public sealed class ProfileEditorFieldEnum<TEnum, TProfile> : SLOptionButton<TEnum>,
//     IProfileEditorField<TEnum, TProfile>
//     where TProfile : class, IPersistentProfile<TProfile>
//     where TEnum : struct, Enum
// {
//     private readonly Func<TEnum, string>? _localizeFunc;
//     private readonly Func<TEnum, bool>? _ignoredEnums;
//     private readonly Func<TProfile, TEnum> _readData;
//     private readonly Action<TProfile, TEnum> _writeData;
//
//     private TProfile _data {
//         get
//         {
//             if (_profile == null)
//                 throw new InvalidOperationException("Profile must be injected!");
//             return _profile;
//         }
//     }
//     private TProfile? _profile = null;
//     public void InjectProfile(TProfile profile)
//     {
//         _profile = profile;
//         profile.OnSync += FromProfile;
//     }
//
//     public ProfileEditorFieldEnum(
//         Func<TProfile, TEnum> readData,
//         Action<TProfile, TEnum> writeData,
//         Func<TEnum, string>? localizeEnum = null,
//         string? locPrefix = null,
//         Func<TEnum, bool>? ignoredEnums = null)
//     {
//         LocPrefix = locPrefix;
//         _localizeFunc = localizeEnum;
//         _ignoredEnums = ignoredEnums;
//         _readData = readData;
//         _writeData = writeData;
//     }
//     public void FromProfile(TProfile profile) => SelectByData(_readData.Invoke(profile));
//
//     public void ToProfile() => _writeData.Invoke(_data, CurrentOption);
//
//     public override IEnumerable<TEnum> EnumerateOptions()
//     {
//         if (_ignoredEnums != null)
//         {
//             foreach (var value in EnumerateOptions())
//             {
//                 if (!_ignoredEnums.Invoke(value)) ;
//                 yield return value;
//             }
//         }
//         else
//             foreach (var value in Enum.GetValues<TEnum>())
//                 yield return value;
//     }
//
//     protected override string GetOptionLabel(TEnum data)
//     {
//         return _localizeFunc != null ? _localizeFunc(data) : data.ToString();
//     }
// }