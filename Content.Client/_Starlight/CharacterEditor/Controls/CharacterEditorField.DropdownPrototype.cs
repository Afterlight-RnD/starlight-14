// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterEditor.Controls;

[Virtual]
public class CharacterEditorDropdownPrototypeField<TProfileData, TPrototype>
    : CharacterEditorDropdownField<TProfileData, TPrototype>
    where TProfileData : CharacterData, new()
    where TPrototype: class, IPrototype
{
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

    public Func<TPrototype,string>? LocalizedNameGetter;

    protected override void EnteredTree()
    {
        base.EnteredTree();
        _prototypeManager.PrototypesReloaded += HandleProtoReload;
    }

    protected override void ExitedTree()
    {
        _prototypeManager.PrototypesReloaded -= HandleProtoReload;
        base.ExitedTree();
    }

    private void HandleProtoReload(PrototypesReloadedEventArgs obj)
    {
        if (!obj.WasModified<TPrototype>())
            return;
        var currentId = new ProtoId<TPrototype>(CurrentOption.ID);
        SetupOptions();
        SelectByData(_prototypeManager.Index(currentId));
    }

    public override IEnumerable<TPrototype> EnumerateOptions()
    {
        foreach (var protoType in _prototypeManager.EnumeratePrototypes<TPrototype>())
            yield return protoType;
    }

    protected override string GetOptionLabel(TPrototype data)
    {
        return LocalizedNameGetter == null ? data.ID: LocalizedNameGetter.Invoke(data);
    }
}