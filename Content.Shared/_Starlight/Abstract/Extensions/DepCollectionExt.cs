// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Abstract.Interfaces;

namespace Content.Shared._Starlight.Abstract.Extensions;

public static class DepCollectionExt
{
    public static void InjectDependencies<T, TDepType>(this IDependencyCollection dependencies, T target,
        bool oneOff = false)
        where T : IInjectDependencies<TDepType>, new()
        where TDepType : class, IDependencyType
    {
        dependencies.InjectDependencies(target, oneOff);
        target.PostInject();
    }

    public static void InjectDependencies<TDepType>(this IDependencyCollection dependencies, object target,
        bool oneOff = false)
        where TDepType : class, IDependencyType
    {
        if (target is not IInjectDependencies<TDepType> depType) return;
        dependencies.InjectDependencies(target, oneOff);
        depType.PostInject();
    }
}