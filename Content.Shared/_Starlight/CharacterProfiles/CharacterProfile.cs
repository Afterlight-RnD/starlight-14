// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Shared._Starlight.CharacterProfiles.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class CharacterProfile
{
    //TODO: probably should have some custom serialization stuff so that types aren't serialized into yaml?
    [DataField] private Dictionary<Type, ICharacterData> _data = new();

    public CharacterProfile(List<ICharacterData> dataList)
    {
        SetFromList(dataList, false);
    }

    private HashSet<Type> _dirtyData = new();

    public List<ICharacterData> GetData(bool onlyDirty = true)
    {
        if (!onlyDirty) return _data.Values.ToList();
        var list = new List<ICharacterData>();
        foreach (var (type, data) in _data)
        {
            if (!_data.ContainsKey(type))
                continue;
            list.Add(data);
        }
        return list;
    }

    public bool HasDirtyData { get; private set; }

    public IEnumerable<ICharacterData> IterateCharacterData()
    {
        foreach (var (_, data) in _data)
            yield return data;
    }

    public T GetData<T>() where T: ICharacterData, new()
    {
        return (T)_data[typeof(T)];
    }

    public void MarkDataDirty<T>() where T : ICharacterData, new()
    {
        _dirtyData.Add(typeof(T));
        HasDirtyData = true;
    }

    public void MarkDirty()
    {
        HasDirtyData = true;
        foreach (var (type, _) in _data)
            _dirtyData.Add(type);
    }

    [Access(typeof(CharacterProfileRegistry), typeof(SharedCharacterProfileSystem))]
    public void ClearDirty()
    {
        _dirtyData.Clear();
        HasDirtyData = false;
    }

    [Access(typeof(CharacterProfileRegistry), typeof(SharedCharacterProfileSystem))]
    public void SetFromList(List<ICharacterData> newData, bool shouldDirty = true)
    {
        if (shouldDirty)
        {
            if (newData.Count > 0)
                HasDirtyData = true;
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

public interface ICharacterData
{
    public Type? GetComponentType { get; }
}

[ImplicitDataDefinitionForInheritors, Serializable, NetSerializable]
public abstract partial class CharacterData : ICharacterData
{
    public Type? GetComponentType => null;
}

[ImplicitDataDefinitionForInheritors, Serializable, NetSerializable]
public abstract partial class CharacterData<T> : ICharacterData where T : IComponent, new()
{
    public Type GetComponentType => typeof(T);
}