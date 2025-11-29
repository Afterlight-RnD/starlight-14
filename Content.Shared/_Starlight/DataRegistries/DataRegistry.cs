// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager;

namespace Content.Shared._Starlight.DataRegistries;

public interface IDataRegistry
{
    public Type EntryBaseType { get; }

    public Guid CreateEntry();

    public void EnsureEntry(Guid id);

    public bool TryGetData<TData>(Guid id, [NotNullWhen(true)] out TData? data)
    where TData: class, IDataEntry, new();

    public IEnumerable<IDataEntry> IterateEntries(Guid id);

    public bool GetDataList(Guid id, [NotNullWhen(true)] out List<IDataEntry>? list);
};


public abstract class DataRegistry<TBaseData> : IDataRegistry
    where TBaseData : class, IDataEntry
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;
    [Dependency] private readonly IReflectionManager _reflectionManager = default!;
    [Dependency] private readonly ISerializationManager _serialization = default!;

    public event Action<DataEntry<TBaseData>>? OnEntryCreated;

    public event Action<DataEntry<TBaseData>>? OnEntryRemoved;


    public Dictionary<Guid, DataEntry<TBaseData>> Entries = new();
    public Type EntryBaseType => typeof(TBaseData);

    public bool TryGetEntry(Guid id, [NotNullWhen(true)] out DataEntry<TBaseData>? profile)
    {
        return Entries.TryGetValue(id, out profile);
    }

    public bool TryGetData<TData>(Guid id, [NotNullWhen(true)] out TData? data)
        where TData: class,TBaseData, new()
    {
        data = null;
        if (!TryGetEntry(id, out var entry))
            return false;
        data = entry.GetData<TData>();
        return true;
    }

    public IEnumerable<IDataEntry> IterateEntries(Guid id)
    {
        if (!TryGetEntry(id, out var entry))
            yield break;
        foreach (var data in entry.IterateData())
            yield return data;
    }

    public bool GetDataList(Guid id, [NotNullWhen(true)] out List<IDataEntry>? list)
    {
        list = null;
        if (!TryGetEntry(id, out var entry))
            return false;
        list = new();
        foreach (var data in entry.IterateData())
            list.Add(data);
        return true;
    }

    Guid IDataRegistry.CreateEntry()
    {
        return CreateEntry().Id;
    }

    void IDataRegistry.EnsureEntry(Guid id)
     {
         EnsureEntry(id);
     }

    public bool RemoveEntry(Guid id)
    {
        if (!Entries.Remove(id, out var existing))
            return false;
        OnEntryRemoved?.Invoke(existing);
        return true;
    }

    public DataEntry<TBaseData> CreateEntry(List<TBaseData>? data = null)
    {
        var newEntry = new DataEntry<TBaseData>(_typeFactory, _reflectionManager);
        Entries.Add(newEntry.Id, newEntry);
        if (data != null)
            newEntry.CopyData(_serialization, data);
        OnEntryCreated?.Invoke(newEntry);
        return newEntry;
    }

    public DataEntry<TBaseData> EnsureEntry(Guid id, List<TBaseData>? data = null)
    {
        if (!TryGetEntry(id, out var entry))
        {
            return CreateEntry(data);
        }
        if (data != null)
            entry.CopyData(_serialization, data);
        return entry;
    }
}
