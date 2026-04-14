// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Collections;
using Robust.Shared.Collections;

namespace Content.Shared._Starlight.Core.ECS;

[Virtual]
public class EntityCollection<TArch>(params ValueList<TArch> instances) : IEntityCollection<TArch>
    where TArch : IEntityArchetype, IEntityArchetype.IBuilder<TArch>
{
    public ValueList<TArch> Instances { get; set; } = instances;
}

public interface IEntityCollection<TArch> : IEntityCollection.IBuilder<TArch>
    where TArch: IEntityArchetype, IEntityArchetype.IBuilder<TArch>
{
}

public interface IEntityCollection
{
    public interface IBuilder;

    public interface IBuilder<TArch> : IBuilder
        where TArch : IEntityArchetype.IBuilder<TArch>, IEntityArchetype
    {
        ValueList<TArch> Instances { get; set; }
    }
}
