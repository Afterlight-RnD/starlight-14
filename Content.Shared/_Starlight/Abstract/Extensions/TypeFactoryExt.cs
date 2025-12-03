// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Abstract.Interfaces;

namespace Content.Shared._Starlight.Abstract.Extensions;

public static class TypeFactoryExtensions
{
    public static T CreateInstance<T>(this IDynamicTypeFactory typeFactory,
        IEntitySystemManager? systemDeps = null,
        bool oneOff = false)
    where T: IInjectDependencies<SystemDependencies>, new()
    {
        var instance = typeFactory.CreateInstance<T>(oneOff, false);
        if (systemDeps == null)
            return instance;
        systemDeps.DependencyCollection.InjectDependencies<T, SystemDependencies>(instance, oneOff);
        return instance;
    }

    public static T CreateInstance<T,TDeps>(this IDynamicTypeFactory typeFactory,
        IDependencyCollection? dependencies = null,
        bool oneOff = false)
        where T: IInjectDependencies<TDeps>,new()
        where TDeps: class, IDependencyType
    {
        var instance = typeFactory.CreateInstance<T>(oneOff, false);
        if (dependencies == null)
            return instance;
        dependencies.InjectDependencies<T, TDeps>(instance, oneOff);
        return instance;
    }

    public static TBase CreateCastInstance<TBase, TDeps>(this IDynamicTypeFactory typeFactory,
        Type type,
        IDependencyCollection? dependencies = null,
        bool oneOff = false)
        where TDeps: class, IDependencyType
    {
        var instance = (TBase)typeFactory.CreateInstance(type, oneOff, false);
        if (dependencies == null || instance is not IInjectDependencies<TDeps> deps)
            return instance;
        dependencies.InjectDependencies(dependencies, oneOff);
        deps.PostInject();
        return instance;
    }
}