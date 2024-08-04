using System.Collections.Generic;

namespace Common.Extensions
{
    public static class LinkedListExtensions
    {
        public static T GetLastAndRemove<T>(this LinkedList<T> linkedList)
        {
            var lastValue = linkedList.Last.Value;
            linkedList.RemoveLast();
            return lastValue;
        }
    }
}