// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Abstract.Extensions;

namespace Content.Shared._Starlight.Abstract.Interfaces;

public interface IInjectDependencies<TDepType>
    where TDepType : class, IDependencyType
{
    public void PostInject(){}
}

public interface IDependencyType
{
    public static abstract bool HasSystems { get; }
    public static abstract bool AssertOnFail { get; }
}

public sealed class GlobalDependencies : IDependencyType
{
    public static bool HasSystems => false;
    public static bool AssertOnFail => true;

    private GlobalDependencies(){}
};

public sealed class SystemDependencies : IDependencyType
{
    public static bool HasSystems => true;
    public static bool AssertOnFail => true;

    private SystemDependencies(){}
}

public static class SLDependencyHelpers
{
    public static void InjectSystemDependencies<T>(T type, IEntitySystemManager entitySystemManager, bool oneOff = false)
    where T: IInjectDependencies<SystemDependencies>
    {
        if (!entitySystemManager.TryGetDependencyCollection(out var dependencies))
        {
            if (SystemDependencies.AssertOnFail)
                throw new InvalidOperationException("Tried inject system dependencies outside of simulation!");
            return;
        }
        dependencies.InjectDependencies(type, oneOff);
    }

    public static void InjectSystemDependencies<T>(T type, IDependencyCollection collection, bool oneOff = false)
        where T: IInjectDependencies<GlobalDependencies>
    {
        collection.InjectDependencies(type, oneOff);
    }
}
