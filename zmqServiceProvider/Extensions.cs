using System;
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
    }
}
