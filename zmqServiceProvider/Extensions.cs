using System;
using System.Linq;
using System.Collections.Generic;

namespace zmqServiceProvider.Extensions
{
    static class ListExtensions
    {
        public static T TakeFirst<T>(this List<T> list)
        {
            T r = list[0];
            list.RemoveAt(0);
            return r;
        }

        public static List<T> TakeRange<T>(this List<T> list, int end)
        {
            List<T> r = list.Take(end).ToList();
            list.RemoveRange(0,end);
            return r;
        }
    }
}
