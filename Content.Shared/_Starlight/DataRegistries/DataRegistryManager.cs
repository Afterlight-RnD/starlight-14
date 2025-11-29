// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Shared.Reflection;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._Starlight.DataRegistries;

public sealed class DataRegistryManager: IPostInjectInit
{
    [Dependency] private readonly IDynamicTypeFactory _typeFactory = default!;
    [Dependency] private readonly IReflectionManager _reflectionManager = default!;

    private Dictionary<Type, IDataRegistry> _dataRegistries = new();
    private Dictionary<Type, IDataRegistry> _dataTypesToRegistry = new();

    public void PostInject()
    {
        foreach (var regType in _reflectionManager.GetAllChildren<IDataRegistry>())
        {
            var newReg = _typeFactory.CreateInstance<IDataRegistry>(regType, false);
            _dataRegistries.Add(regType, newReg);
            foreach (var dataType in _reflectionManager.GetAllChildren(newReg.EntryBaseType))
                _dataTypesToRegistry.Add(dataType, newReg);
        }
    }

    public DataRegistry<TBaseData> GetRegistry<TBaseData>()
    where TBaseData: class,IDataEntry
    {
        var type = typeof(TBaseData);
        if (_dataRegistries.TryGetValue(type, out var registry) || _dataTypesToRegistry.TryGetValue(type, out registry))
            return (DataRegistry<TBaseData>)registry;
        throw new GenericParameterMismatchException();
    }
}
