// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles.Systems;
using Robust.Shared.Serialization;

namespace Content.Shared._Starlight.CharacterProfiles;

[DataDefinition, Serializable, NetSerializable]
public sealed partial class CharacterProfile
{
    [DataField] public int Slot;

    //TODO: probably should have some custom serialization stuff so that types aren't serialized into yaml?
    private List<Type> _dataTypes = new();

    [DataField] private List<ICharacterData> _data = new();

    [Access(typeof(SharedCharacterProfileSystem))]

    [DataField]
    public bool Activate { get; set; }
    public bool HasDirtyData => _dirtyData.Count > 0 || _fullDirty;

    [Access(typeof(SharedCharacterProfileSystem))]
    public bool HasInvalidData { get; set; } = false;

    private bool _fullDirty = false;
    private HashSet<Type> _dirtyData = new();

    public CharacterProfile(CharacterProfile other)
    {
        SetData(other.GetData(), false);
    }

    public CharacterProfile(List<ICharacterData> dataList)
    {
        SetData(dataList, false);
    }

    public List<ICharacterData> GetData(bool onlyDirty = true)
    {
        if (!onlyDirty || _fullDirty) return _data;
        var list = new List<ICharacterData>();
        foreach (var data in _data)
        {
            if (_dirtyData.Contains(data.GetType()))
                list.Add(data);
        }
        return list;
    }


    public IEnumerable<ICharacterData> IterateCharacterData()
    {
        foreach (var  data in _data)
            yield return data;
    }

    public T GetData<T>() where T: struct, ICharacterData
    {
        return (T)_data[_dataTypes.IndexOf(typeof(T))];
    }

    public void SetData<T>(T data, bool dirty = true) where T: struct, ICharacterData
    {
        _data[_dataTypes.IndexOf(typeof(T))] = data;

        if (!dirty) return;
        _dirtyData.Add(typeof(T));
    }

    public bool EditData<TData>(CharacterDataSetterDelegate<TData> setterDelegate) where TData: struct, ICharacterData
    {
        var dataIdx = _dataTypes.IndexOf(typeof(TData));
        var data = (TData)_data[dataIdx];
        var dirty = setterDelegate.Invoke(ref data);
        _data[dataIdx] = data;
        if (dirty)
            _dirtyData.Add(typeof(TData));
        return dirty;
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

        void UpdateData(Type dataType, ICharacterData data)
        {
            var dataIdx = _dataTypes.IndexOf(dataType);
            if (dataIdx == -1)
            {
                _data.Add(data);
                _dataTypes.Add(dataType);
                return;
            }
            _data[dataIdx] = data;
        }

        if (shouldDirty)
        {
            foreach (var data in newData)
            {
                var dataType = data.GetType();
                UpdateData(dataType, data);
                _dirtyData.Add(dataType);
            }
            return;
        }
        foreach (var data in newData)
        {
            var dataType = data.GetType();
            UpdateData(dataType, data);
        }
    }
};

public interface ICharacterData;

public delegate bool CharacterDataSetterDelegate<TData>(ref TData data);