// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Collections;
using Robust.Shared.Collections;

namespace Content.Shared._Starlight.Core.ECS;


[Virtual]
public class EntityCollection<TArchetype>(params TArchetype[] archetypes): IEnumerable<TArchetype>,
    IEntityCollection<TArchetype>
    where TArchetype : struct, IEntityArchetype
{
    private readonly ValueList<TArchetype> _data = new(archetypes);
    public int Count => _data.Count;

    public TArchetype this[int index]
    {
        get => _data[index];
        set => _data[index] = value;
    }

    public IEnumerator<TArchetype> GetEnumerator() => _data.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}


public interface IEntityCollection<TArchetype> : IEntityArchetype
    where TArchetype: IEntityArchetype
{
}
public interface IEntityCollection;


public static class EntityCollection
{
    extension<TArchetype, TComp>(EntityCollection<TArchetype> collection) where TArchetype : struct, IEntityArchetype, IEntityArchetype.IComp<TComp>
        where TComp : IComponent
    {
        public IEnumerator<TComp> GetComp()
        {
            foreach (var arch in collection) yield return arch.GetComp;
        }
    }

}
