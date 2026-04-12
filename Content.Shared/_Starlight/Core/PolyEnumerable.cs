// SPDX-FileCopyrightText: 2026 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Shared._Starlight.Core;

#region Enumerable

public readonly ref partial struct PolyEnumerable<TInner, TOuter, TInnerEnumerable>(TInnerEnumerable enumerable);

public static partial class EnumerationHelpers
{
    extension<TInner, TOuter, TSelf> (TSelf self)
        where TSelf : IEnumerable<TInner>, allows ref struct
        where TOuter: TInner
    {
        public PolyEnumerable<TInner, TOuter, TSelf> AsPolyEnumerable() => new(self);

        public PolyEnumerable<TInner, TOuter, TSelf>.Instance AsBase() => new (self);
    }

}

#endregion

