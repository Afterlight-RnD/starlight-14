// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;

namespace Content.Shared._Starlight.Abstract.Extensions;

public static class EntityManagerExt
{
    public static bool TryGetDependencyCollection(this IEntityManager entityManager, [NotNullWhen(true)] out IDependencyCollection? systemDeps)
    {
        return entityManager.EntitySysManager.TryGetDependencyCollection(out systemDeps);
    }

    public static bool IsInitialized(this IEntityManager entityManager)
    {
        return entityManager.EntitySysManager.IsInitialized();
    }
}