// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Shared._Starlight.CharacterProfiles.Data;
using Content.Shared._Starlight.CharacterProfiles.Systems;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class CharacterProfile
{
    [DataField] public int Slot;

    //TODO: probably should have some custom serialization stuff so that types aren't serialized into yaml?
    [DataField] private Dictionary<Type, ICharacterData> _data = new();

    [Access(typeof(SharedCharacterProfileSystem))]
    [DataField] public EntProtoId DollPrototype { get; set; }

    [Access(typeof(SharedCharacterProfileSystem))]

    [DataField] public bool Activate { get; set; }
    public bool HasDirtyData => _dirtyData.Count > 0 || _fullDirty;

    [Access(typeof(SharedCharacterProfileSystem))]
    public bool HasInvalidData { get; set; } = false;

    private bool _fullDirty = false;
    private HashSet<Type> _dirtyData = new();

    public CharacterProfile(CharacterProfile other)
    {
        SetData(other.GetData(), false);

        //TODO: Legacy migration
        DollPrototype = IoCManager.Resolve<IPrototypeManager>()
            .Index(GetData<LegacyCharacterData>().LegacyProfile.Species).DollPrototype;
    }

    public CharacterProfile(List<ICharacterData> dataList)
    {
        SetData(dataList, false);

        //TODO: Legacy migration
        DollPrototype = IoCManager.Resolve<IPrototypeManager>()
            .Index(GetData<LegacyCharacterData>().LegacyProfile.Species).DollPrototype;
    }

    public Dictionary<Type, ICharacterData> GetTypedData(bool onlyDirty = true)
    {
        if (!onlyDirty || _fullDirty) return _data;
        var list = new Dictionary<Type, ICharacterData>();
        foreach (var (type, data) in _data)
        {
            if (!_data.ContainsKey(type))
                continue;
            list.Add(type, data);
        }

        return list;
    }

    public List<ICharacterData> GetData(bool onlyDirty = true)
    {
        if (!onlyDirty || _fullDirty) return _data.Values.ToList();
        var list = new List<ICharacterData>();
        foreach (var (type, data) in _data)
        {
            if (!_data.ContainsKey(type))
                continue;
            list.Add(data);
        }
        return list;
    }



    public IEnumerable<ICharacterData> IterateCharacterData()
    {
        foreach (var (_, data) in _data)
            yield return data;
    }

    public T GetData<T>() where T: struct, ICharacterData
    {
        return (T)_data[typeof(T)];
    }

    public void SetData<T>(T data, bool dirty = true) where T: struct, ICharacterData
    {
        _data[typeof(T)] = data;

        if (!dirty) return;
        _dirtyData.Add(typeof(T));
    }

    public void MarkDirty()
    {
        _fullDirty = true;
    }

    [Access(typeof(CharacterProfileRegistry),
        typeof(SharedCharacterProfileSystem),
        typeof(MsgUpdateCharacterProfile),
        typeof(MsgSyncCharacterProfile))]
    public void ClearDirty()
    {
        _dirtyData.Clear();
        _fullDirty = false;
    }

    public void SetData(List<ICharacterData> newData, bool shouldDirty = true)
    {
        if (shouldDirty)
        {
            foreach (var data in newData)
            {
                var dataType = data.GetType();
                _data.Add(dataType, data);
                _dirtyData.Add(dataType);
            }
            return;
        }
        foreach (var data in newData)
        {
            var dataType = data.GetType();
            _data.Add(dataType, data);
        }
    }
};

public interface ICharacterData;