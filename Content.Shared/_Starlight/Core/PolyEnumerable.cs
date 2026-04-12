// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Collections;
using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._Starlight.Core;

public static partial class EnumerationHelpers
{
    extension<TInner, TInnerEnumerable, TSelf>(TSelf enumerable)
    where TSelf: IPolyEnumerable<TSelf>, IPolyEnumerable<TInner, TSelf>,
    IEnumerable<TInner>
    where TInnerEnumerable : IEnumerable<TInner>
    {
        private TInnerEnumerable GetInnerEnum() => TSelf.AsInnerEnumerable<TInnerEnumerable>(enumerable);
    }

    extension<TInner, TInnerEnumerable, TSelf>(TSelf enumerable)
        where TSelf: IPolyEnumerable<TSelf>, IPolyEnumerable<TInner, TSelf>,
        IEnumerable<TInner>
        where TInnerEnumerable : class, IEnumerable<TInner>
    {
        private TInnerEnumerable? GetInnerEnumSafe() => TSelf.AsInnerEnumerableSafe<TInnerEnumerable>(enumerable);
    }

    public interface IPolyEnumerableBase
    {
    }

    public interface IPolyEnumerable<in TSelf>
        : IPolyEnumerableBase
        where TSelf: IPolyEnumerable<TSelf>
    {
    }

    public interface IPolyEnumerable<TInner, in TSelf> :
        IPolyEnumerableBase, IEnumerator<TInner>
        where TSelf: IPolyEnumerable<TInner, TSelf>
    {
        static virtual TInnerEnum AsInnerEnumerable<TInnerEnum>(TSelf self) where TInnerEnum: IEnumerable<TInner> => throw new GenericParameterMismatchException();
        static virtual TInnerEnum? AsInnerEnumerableSafe<TInnerEnum>(TSelf self) where TInnerEnum: class, IEnumerable<TInner> => null;
    }

    public interface IPolyEnumerable<TInner, in TOuter, in TInnerEnumerable,in TSelf> :
        IPolyEnumerable<TInner, TSelf>, IPolyEnumerable<TSelf>
        where TOuter : TInner
        where TSelf : IPolyEnumerable<TInner, TOuter,TInnerEnumerable,TSelf>, IEnumerable<TInner>,IPolyEnumerable<TSelf>
        where TInnerEnumerable: IEnumerable<TInner>
    {
        static new virtual TInnerEnum AsInnerEnumerable<TInnerEnum>(TSelf self)
            where TInnerEnum: TInnerEnumerable
            => self.GetInnerEnum<TInner, TInnerEnum, TSelf>();

        static new virtual TInnerEnum? AsInnerEnumerableSafe<TInnerEnum>(TSelf self)
            where TInnerEnum: class, IEnumerable<TInner>
            => self.GetInnerEnumSafe<TInner, TInnerEnum, TSelf>();
    }
}



