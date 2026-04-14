// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
namespace Content.Shared._Starlight.Core.ECS;

public readonly partial struct EntityArchetype<TSelf> (EntityUid id): IEntityArchetype<TSelf>
    where TSelf : IEntityArchetype<TSelf>, IEntityArchetype.IBuilder
{
    public EntityUid Uid { get; init; } = id;
};

[ImplicitDataDefinitionForInheritors]
public partial interface IEntityArchetype
{
    EntityUid Uid { get; init; }

    public interface IBuilder
    {
    }

    public interface IBuilder<TSelf> : IBuilder
        where TSelf : IEntityArchetype, IBuilder<TSelf>;

    public interface IBuilder<TComp, TSelf> : IBuilder<TSelf>
        where TSelf : IEntityArchetype, IBuilder<TComp, TSelf>
        where TComp : IComponent
    {
    }
}

public partial interface IEntityArchetype<TSelf> : IEntityArchetype, IEntityArchetype.IBuilder<TSelf>
    where TSelf : IEntityArchetype.IBuilder<TSelf>, IEntityArchetype
{
};
