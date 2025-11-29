// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._Starlight.DataRegistries;


[ImplicitDataDefinitionForInheritors]
public partial interface IDataEntry
{
}

[DataDefinition]
public sealed partial class DataEntry<TBaseData>
where TBaseData: class, IDataEntry
{
    public Guid Id { get; set; } = Guid.Empty;
    public bool IsValid => Id != Guid.Empty;
    private Dictionary<Type, TBaseData> _data = new();

    public DataEntry(IDynamicTypeFactory typeFactory, IReflectionManager reflectionManager, Guid? idOverride = null)
    {
        idOverride ??= Guid.NewGuid();
        Id = idOverride.Value;
        foreach (var type in reflectionManager.GetAllChildren<IDataEntry>())
        {
            _data.Add(type, typeFactory.CreateInstance<TBaseData>(type));
        }
    }

    public DataEntry(List<TBaseData> entryData, Guid? idOverride = null)
    {
        idOverride ??= Guid.NewGuid();
        Id = idOverride.Value;
        foreach (var data in entryData)
        {
            _data.Add(data.GetType(), data);
        }
    }

    public IEnumerable<TBaseData> IterateData()
    {
        foreach (var (_, data) in _data)
        {
            yield return data;
        }
    }

    public List<TBaseData> GetData()
    {
        return _data.Values.ToList();
    }

    public TData GetData<TData>() where TData: TBaseData, new()
    {
        return (TData)_data[typeof(TData)];
    }

    public void CopyData<TData>(ISerializationManager serialization, TData data) where TData : TBaseData, new()
    {
        var target = GetData<TData>();
        serialization.CopyTo(data, ref target);
    }

    public void CopyData<TData>(ISerializationManager serialization, List<TData> data) where TData : TBaseData
    {
        foreach (var entry in data)
        {
            var target = (TData)_data[entry.GetType()];
            serialization.CopyTo(entry, ref target);
        }
    }

    public void CopyFrom(ISerializationManager serialization, DataEntry<TBaseData> other)
    {
        foreach (var entry in _data)
        {
            var target = entry.Value;
            serialization.CopyTo(other._data[entry.Key], ref target);
        }
    }
}
