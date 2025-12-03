// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;

namespace Content.Shared._Starlight.Abstract.Extensions;

public static class EntitySystemManagerExt
{
    public static bool TryGetDependencyCollection(this IEntitySystemManager systemManager, [NotNullWhen(true)] out IDependencyCollection? systemDeps)
    {
        //This is the only way to check if entitySystemManager is initialized... Why isn't this a boolean property... FML
        try
        {
            systemDeps = systemManager.DependencyCollection;
        }
        catch (InvalidOperationException e)
        {
            systemDeps = null;
            return false;
        }
        return true;
    }

    public static bool IsInitialized(this IEntitySystemManager systemManager)
    {
        //This is the only way to check if entitySystemManager is initialized... Why isn't this a boolean property... FML
        try
        {
            var systemDeps = systemManager.DependencyCollection;
        }
        catch (InvalidOperationException e)
        {
            return false;
        }
        return true;
    }
}