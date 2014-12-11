using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUpdater
{
    public static class ListExtension
    {
        public static void AddDistinct<T>(this IList<T> list, IEnumerable<T> collection) where T : IEquatable<T>
        {
            foreach (var c in collection)
                if (!list.Contains(c))
                    list.Add(c);
        }
    }
}
