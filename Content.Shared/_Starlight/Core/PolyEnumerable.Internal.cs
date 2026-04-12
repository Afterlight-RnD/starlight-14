// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Collections;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._Starlight.Core;

public readonly ref partial struct PolyEnumerable<TInner, TOuter, TInnerEnumerable> : EnumerationHelpers.IPolyEnumerable<TInner, PolyEnumerable<TInner, TOuter, TInnerEnumerable>>,
    IEnumerable<TInner>, IEnumerator<PolyEnumerable<TInner, TOuter, TInnerEnumerable>.Instance>
    where TOuter : TInner, allows ref struct
    where TInnerEnumerable : IEnumerable<TInner>, allows ref struct
{

    public readonly Instance Enumerator = new(enumerable);

    public bool MoveNext() => Enumerator.MoveNext();

    public void Reset() => Enumerator.Reset();
    public Instance Current => Enumerator;
    TInner IEnumerator<TInner>.Current => Enumerator.Current;

    object? IEnumerator.Current => Enumerator.Current;

    public void Dispose() => Enumerator.Dispose();

    public IEnumerator<TInner> GetEnumerator() => Enumerator.Get;

    IEnumerator IEnumerable.GetEnumerator() => Enumerator.Get;

    public readonly ref struct Instance(TInnerEnumerable inner) : IEnumerator<TInner>
    {
        private readonly IEnumerator<TInner> _inner = inner.GetEnumerator();

        public bool MoveNext() => _inner.MoveNext();

        public void Reset() => _inner.Reset();

        TInner IEnumerator<TInner>.Current => _inner.Current;

        object? IEnumerator.Current => _inner.Current;

        public TInner Current => _inner.Current;

        public IEnumerator<TInner> Get => _inner;

        public void Dispose() => _inner.Dispose();
    }
}

public static partial class EnumerationHelpers
{
    public interface IPolyEnumerableApi<TEnumerable> : IPolyEnumerable
        where TEnumerable: IDisposable, IEnumerator, allows ref struct;

    public interface IPolyEnumerable;

    public interface IPolyEnumerableBase
    {
    }

    public interface IPolyEnumerable<in TSelf>
        : IPolyEnumerableBase
        where TSelf: IPolyEnumerable<TSelf>, allows ref struct
    {
    }

    public interface IPolyEnumerable<TInner, in TSelf> :
        IPolyEnumerableBase, IEnumerator<TInner>
        where TSelf: IPolyEnumerable<TInner, TSelf>, allows ref struct
    {
        static virtual TInnerEnum AsInnerEnumerable<TInnerEnum>(TSelf self) where TInnerEnum: IEnumerable<TInner> => throw new GenericParameterMismatchException();
        static virtual TInnerEnum? AsInnerEnumerableSafe<TInnerEnum>(TSelf self) where TInnerEnum: class, IEnumerable<TInner> => null;
    }

    public interface IPolyEnumerable<TInner, in TOuter, in TInnerEnumerable, TSelf> :
        IPolyEnumerable<TInner, TSelf>, IPolyEnumerable<TSelf>
        where TOuter : TInner, allows ref struct
        where TSelf : TInnerEnumerable, IPolyEnumerable<TInner, TOuter,TInnerEnumerable,TSelf>, IEnumerable<TInner>, IPolyEnumerable<TInner, TOuter, TSelf, TSelf>, allows ref struct
        where TInnerEnumerable: IDisposable, IEnumerator, IEnumerable<TInner>, allows ref struct
    {
        abstract static TSelf Make(TInnerEnumerable enumerable);


        static new virtual TInnerEnum AsInnerEnumerable<TInnerEnum>(TSelf self)
            where TInnerEnum: TInnerEnumerable
            => self.GetInnerEnumInternal<TInner, TInnerEnum, TSelf>();

        static new virtual TInnerEnum? AsInnerEnumerableSafe<TInnerEnum>(TSelf self)
            where TInnerEnum: class, IEnumerable<TInner>
            => self.GetInnerEnumSafe<TInner, TInnerEnum, TSelf>();
    }


    #region Extension BoilerPlate
    extension<TInner, TInnerEnumerable, TSelf>(TSelf enumerable)
        where TSelf: IPolyEnumerable<TSelf>, IPolyEnumerable<TInner, TSelf>,
        IEnumerable<TInner>, allows ref struct
        where TInnerEnumerable : IEnumerable<TInner>
    {
        private TInnerEnumerable GetInnerEnumInternal() => TSelf.AsInnerEnumerable<TInnerEnumerable>(enumerable);
    }

    extension<TInner, TInnerEnumerable, TSelf>(TSelf enumerable)
        where TSelf: IPolyEnumerable<TSelf>, IPolyEnumerable<TInner, TSelf>,
        IEnumerable<TInner>, allows ref struct
        where TInnerEnumerable : class, IEnumerable<TInner>
    {
        private TInnerEnumerable? GetInnerEnumSafe() => TSelf.AsInnerEnumerableSafe<TInnerEnumerable>(enumerable);
    }
    #endregion
}



