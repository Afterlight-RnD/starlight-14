// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
namespace Content.Shared._Starlight.Core.ECS;


public readonly struct EntityArchetype<TComp,TSelf>(TSelf self, TComp comp) : IEntityArchetype<TComp,TSelf>
    where TSelf : struct, IEntityArchetype.IComp<TComp, TSelf>, IEntityArchetype.IComp<TComp>
    where TComp : IComponent
{
    public TComp GetComp => comp;
    public TSelf GetSelf => self;
}

public readonly struct EntityArchetype<TSelf>(TSelf self)
    where TSelf : struct, IEntityArchetype.IComp
{
    public TSelf GetSelf => self;
}

public interface IEntityArchetype<TComp, TSelf> : IEntityArchetype.IComp<TComp, TSelf>
    where TComp : IComponent
    where TSelf : IEntityArchetype.IComp<TComp, TSelf>,
    IEntityArchetype.IComp<TComp>;

public interface IEntityArchetype
{
    interface IComp<TComp, TSelf> : IComp
        where TComp : IComponent
        where TSelf : IComp<TComp, TSelf>, IComp<TComp>, IComp
    {
        TSelf GetSelf { get; }

        readonly struct Comp(TSelf self)
        {
            public TSelf GetSelf => self;
        }
    }

    interface IComp<TComp> : IComp
        where TComp : IComponent
    {
        TComp GetComp { get; }

        readonly struct Comp(TComp self)
            : IComp<TComp>
        {
            public TComp GetComp => self;
        }
    }

    interface IComp;
}
