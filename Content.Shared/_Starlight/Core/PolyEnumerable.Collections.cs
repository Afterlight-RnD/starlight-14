// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Shared.Serialization.Manager.Exceptions;

namespace Content.Shared._Starlight.Core;

public static partial class EnumerationHelpers
{
    #region ICollection
    public interface IPolyCollection<TInner> : IPolyEnumerableBase, ICollection<TInner>;

    public interface IPolyCollection<TInner, in TSelf> : IPolyCollection<TInner>
        where TSelf : IPolyCollection<TInner, TSelf>, ICollection<TInner>
    {
        static virtual TInnerCollection AsInnerCollection<TInnerCollection>(TSelf self)
            where TInnerCollection : ICollection<TInner> => throw new GenericParameterMismatchException();

        static virtual TInnerCollection? AsInnerCollectionSafe<TInnerCollection>(TSelf self)
            where TInnerCollection : class, ICollection<TInner> => null;
    }

    public interface IPolyCollection<TInner, in TOuter, in TInnerCollection, in TSelf>
        : IPolyEnumerable<TInner, TOuter, TInnerCollection, TSelf>, IPolyCollection<TInner, TSelf>
        where TSelf : IPolyCollection<TInner, TOuter, TInnerCollection, TSelf>,
        IPolyEnumerable<TSelf>, IPolyCollection<TInner, TSelf>
        where TInnerCollection : ICollection<TInner>
        where TOuter : TInner;

    #region BoilerPlate Extensions

    extension<TInner, TInnerCollection, TSelf>(TSelf collection)
        where TSelf : IPolyCollection<TInner>, IPolyCollection<TInner, TSelf>,
        ICollection<TInner>
        where TInnerCollection : ICollection<TInner>
    {
        private TInnerCollection GetInnerCollection() => TSelf.AsInnerCollection<TInnerCollection>(collection);
    }

    extension<TInner, TInnerCollection, TSelf>(TSelf collection)
        where TSelf : IPolyCollection<TInner>, IPolyCollection<TInner, TSelf>,
        ICollection<TInner>
        where TInnerCollection : class, ICollection<TInner>
    {
        private TInnerCollection? GetInnerCollectionSafe() => TSelf.AsInnerCollectionSafe<TInnerCollection>(collection);
    }

        #endregion

    #endregion

    #region List

    public interface IPolyList<TInner> : IPolyEnumerableBase, IList<TInner>;

    public interface IPolyList<TInner, in TOuter, in TList, in TSelf> :
        IPolyCollection<TInner, TOuter, TList, TSelf>, IPolyList<TInner>
        where TSelf : IPolyCollection<TInner, TOuter, TList, TSelf>
        where TList : IList<TInner>
        where TOuter : TInner;

    #region BoilerPlate Extensions

    extension<TInner, TList, TSelf>(TSelf enumerable)
        where TSelf : IPolyList<TInner>, IPolyCollection<TInner, TSelf>,
        ICollection<TInner>
        where TList : IList<TInner>
    {
        private TList GetInnerList() => TSelf.AsInnerCollection<TList>(enumerable);
    }

    extension<TInner, TList, TSelf>(TSelf enumerable)
        where TSelf : IPolyList<TInner>, IPolyCollection<TInner, TSelf>,
        ICollection<TInner>
        where TList : class, IList<TInner>
    {
        private TList? GetInnerListSafe() => TSelf.AsInnerCollectionSafe<TList>(enumerable);
    }

        #endregion
    #endregion

    #region Set

    public interface IPolySet<TInner> : IPolyCollection<TInner>, ISet<TInner>;

    public interface IPolySet<TInner, in TOuter, in TSet, in TSelf> :
        IPolyCollection<TInner, TOuter, TSet, TSelf>, IPolySet<TInner>
        where TSelf : IPolyCollection<TInner, TOuter, TSet, TSelf>
        where TSet : ISet<TInner>
        where TOuter : TInner;

    #region BoilerPlate Extensions

    extension<TInner, TSet, TSelf>(TSelf enumerable)
        where TSelf : IPolySet<TInner>, IPolyCollection<TInner, TSelf>,
        ISet<TInner>
        where TSet : ISet<TInner>
    {
        private TSet GetInnerSet() => TSelf.AsInnerCollection<TSet>(enumerable);
    }

    extension<TInner, TSet, TSelf>(TSelf enumerable)
        where TSelf : IPolySet<TInner>, IPolyCollection<TInner, TSelf>,
        ISet<TInner>
        where TSet : class, ISet<TInner>
    {
        private TSet? GetInnerSetSafe() => TSelf.AsInnerCollectionSafe<TSet>(enumerable);
    }

    #endregion

    #endregion

    //TODO: dictionary
}
