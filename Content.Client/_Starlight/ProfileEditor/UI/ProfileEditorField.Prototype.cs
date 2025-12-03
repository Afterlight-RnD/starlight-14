// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.ProfileEditor.UI;

// public partial interface IProfileEditorFieldBuilder<TProfile>
// {
//     public void ProfileEditorFieldProto<TProto>(
//         ProfileEditorField<TProfile> fieldControl,
//         IPrototypeManager protoManager,
//         Func<TProfile,  ProtoId<TProto>> readData,
//         Action<TProfile,  ProtoId<TProto>> writeData,
//         Func<TProto, string> getOptionLabel,
//         Func<TProto, bool>? ignoredPrototypes = null)
//         where TProto: class,IPrototype
//     {
//         RegisterField(fieldControl,
//             new ProfileEditorFieldPrototype<TProto, TProfile>(protoManager, readData, writeData, getOptionLabel, ignoredPrototypes));
//     }
// }
//
//
// public sealed class ProfileEditorFieldPrototype<TProto,TProfile> : SLOptionButton<ProtoId<TProto>>, IProfileEditorField<TProto, TProfile>
// where TProto: class,IPrototype
// where TProfile : class, IPersistentProfile<TProfile>
// {
//     private IPrototypeManager _prototypeManager = default!;
//
//     private Func<TProto, string> _getOptionLabel;
//     private Func<TProto, bool>? _ignoredProtos;
//     private readonly Func<TProfile,  ProtoId<TProto>> _readData;
//     private readonly Action<TProfile, ProtoId<TProto>> _writeData;
//
//     private TProfile _data
//     {
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
//     public ProfileEditorFieldPrototype(
//         IPrototypeManager protoManager,
//         Func<TProfile,  ProtoId<TProto>> readData,
//         Action<TProfile,  ProtoId<TProto>> writeData,
//         Func<TProto, string> getOptionLabel,
//         Func<TProto, bool>? ignoredPrototypes = null)
//     {
//         _prototypeManager = protoManager;
//         _getOptionLabel = getOptionLabel;
//         _ignoredProtos = ignoredPrototypes;
//         _readData = readData;
//         _writeData = writeData;
//     }
//
//     protected override void EnteredTree()
//     {
//         base.EnteredTree();
//         _prototypeManager.PrototypesReloaded += HandleReload;
//     }
//
//     protected override void ExitedTree()
//     {
//         base.ExitedTree();
//         _prototypeManager.PrototypesReloaded -= HandleReload;
//     }
//
//     private void HandleReload(PrototypesReloadedEventArgs obj)
//     {
//         ClearOptions();
//         SetupOptions();
//     }
//
//     public override IEnumerable<ProtoId<TProto>> EnumerateOptions()
//     {
//         if (_ignoredProtos != null)
//         {
//             foreach (var proto in _prototypeManager.EnumeratePrototypes<TProto>())
//             {
//                 if (!_ignoredProtos.Invoke(proto))
//                     yield return proto.ID;
//             }
//         }
//         else
//             foreach (var proto in _prototypeManager.EnumeratePrototypes<TProto>())
//                 yield return proto.ID;
//     }
//     protected override string GetOptionLabel(ProtoId<TProto> data) => _getOptionLabel(_prototypeManager.Index(data));
//
//     public void FromProfile(TProfile profile) => SelectByData(_readData.Invoke(profile));
//
//     public void ToProfile() => _writeData.Invoke(_data, CurrentOption);
// }