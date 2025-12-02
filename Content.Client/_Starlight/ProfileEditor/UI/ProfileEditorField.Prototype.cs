// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Controls;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.ProfileEditor.UI;

public sealed class ProfileEditorPrototypeField<TProto,TProfile> : SLOptionButton<ProtoId<TProto>>, IProfileEditorField<TProto, TProfile>
where TProto: class,IPrototype
where TProfile : IPersistentProfile, new()
{
    public IProfileEditor<TProfile> Editor { get; }

    private IPrototypeManager _prototypeManager = default!;

    private Func<TProto, string> _getOptionLabel;
    private Func<TProto, bool>? _ignoredProtos;
    private readonly Func<TProfile,  ProtoId<TProto>> _readData;
    private readonly Action<TProfile, ProtoId<TProto>> _writeData;

    public ProfileEditorPrototypeField(
        IProfileEditor<TProfile> editor,
        IPrototypeManager protoManager,
        Func<TProfile,  ProtoId<TProto>> readData,
        Action<TProfile,  ProtoId<TProto>> writeData,
        Func<TProto, string> getOptionLabel,
        Func<TProto, bool>? ignoredPrototypes = null)
    {
        Editor = editor;
        _prototypeManager = protoManager;
        _getOptionLabel = getOptionLabel;
        _ignoredProtos = ignoredPrototypes;
        _readData = readData;
        _writeData = writeData;
    }

    protected override void EnteredTree()
    {
        base.EnteredTree();
        _prototypeManager.PrototypesReloaded += HandleReload;
    }

    protected override void ExitedTree()
    {
        base.ExitedTree();
        _prototypeManager.PrototypesReloaded -= HandleReload;
    }

    private void HandleReload(PrototypesReloadedEventArgs obj)
    {
        ClearOptions();
        SetupOptions();
    }

    public override IEnumerable<ProtoId<TProto>> EnumerateOptions()
    {
        if (_ignoredProtos != null)
        {
            foreach (var proto in _prototypeManager.EnumeratePrototypes<TProto>())
            {
                if (!_ignoredProtos.Invoke(proto))
                    yield return proto.ID;
            }
        }
        else
            foreach (var proto in _prototypeManager.EnumeratePrototypes<TProto>())
                yield return proto.ID;
    }
    protected override string GetOptionLabel(ProtoId<TProto> data) => _getOptionLabel(_prototypeManager.Index(data));

    public void FromProfile(TProfile data) => SelectByData(_readData.Invoke(data));

    public void ToProfile(TProfile data) => _writeData.Invoke(data, CurrentOption);
}