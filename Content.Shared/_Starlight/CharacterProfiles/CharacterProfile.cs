// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Shared._Starlight.CharacterProfiles.Systems;
using Robust.Shared.Serialization;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._Starlight.CharacterProfiles;

public interface IPersistentProfile;


[DataDefinition]
public sealed partial class CharacterProfile : IPersistentProfile
{
    [Dependency] private readonly ISerializationManager _serMan = default!;
    [DataField] public int Slot;

    [DataField] private Dictionary<Type,CharacterData> _data = new();

    [Access(typeof(SharedCharacterProfileSystem))]

    [DataField]
    public bool Active { get; set; }

    public bool HasDirtyData
    {
        get
        {
            foreach (var (_,data) in _data)
            {
                if (data.IsDirty)
                    return true;
            }
            return false;
        }
    }

    [Access(typeof(SharedCharacterProfileSystem))]
    public bool HasInvalidData { get; set; } = false;

    public CharacterProfile(CharacterProfile other)
    {
        IoCManager.InjectDependencies(this);
        foreach (var (type, data) in other._data)
            _data.Add(type, _serMan.CreateCopy(data, notNullableOverride:true));
    }

    public CharacterProfile(List<CharacterData> dataList)
    {
        IoCManager.InjectDependencies(this);
        foreach (var data in dataList)
            _data.Add(data.GetType(), data);
    }


    public void SetData(List<CharacterData> dataList)
    {
        foreach (var newData in dataList)
        {
            var newDataType = newData.GetType();
            if (_data.TryGetValue(newDataType, out var data))
            {
                _serMan.CopyTo(newData, ref data, notNullableOverride:true);
                continue;
            }
            _data.Add(newDataType,newData);
        }
    }

    public List<CharacterData> GetData(bool onlyDirty = true)
    {
        if (!onlyDirty) return _data.Values.ToList();
        var list = new List<CharacterData>();
        foreach (var (_,data) in _data)
            if (data.IsDirty)
                list.Add(data);
        return list;
    }

    public IEnumerable<CharacterData> IterateCharacterData()
    {
        foreach (var  (_,data) in _data)
            yield return data;
    }

    public T GetData<T>() where T : CharacterData, new()
    {
        return (T)_data[typeof(T)];
    }

    public void MarkDirty()
    {
        foreach (var (_,data) in _data)
            data.IsDirty = true;
    }

    [Access(typeof(CharacterProfileRegistry),
        typeof(SharedCharacterProfileSystem),
        typeof(MsgUpdateCharacterProfile),
        typeof(MsgSyncCharacterProfile))]
    public void ClearDirty()
    {
        foreach (var (_,data) in _data)
            data.IsDirty = false;
    }
};

[DataDefinition, NetSerializable, Serializable]
public abstract partial class CharacterData
{
    [Access(typeof(CharacterProfile))] public bool IsDirty { get => _dirty; set => _dirty = value; }
    [NonSerialized] private bool _dirty;

    public void Dirty() { _dirty = true; }
}

public delegate void CharacterDataSetterDelegate<in TCharacterData, in TValue>(TValue value, CharacterProfile profile, TCharacterData profileData)
    where TCharacterData: CharacterData, new();

public delegate TValue CharacterDataGetterDelegate<in TCharacterData, out TValue>(TCharacterData data)
    where TCharacterData: CharacterData, new();