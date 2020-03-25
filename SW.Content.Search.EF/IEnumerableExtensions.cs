using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SW.Content.Search.EF
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<IEnumerable<TTo>> ToBatch<TFrom,TTo>(this IEnumerable<TFrom> source, int batchSize, Func<TFrom,TTo> selector)
        {
            var count = source.Count();
            return Enumerable.Range(0, count / batchSize + ((count % batchSize > 0) ? 1 : 0))
                .Select(i => source
                    .Skip(i * batchSize)
                    .Take(batchSize)
                    .Select(selector));
        }
    }
}
