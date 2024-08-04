using System;

namespace Common.Extensions
{
    public static class DataTimeExtensions
    {
        public static DateTime GetMax(DateTime a, DateTime b)
        {
            return a > b ? a : b;
        }

        public static DateTime GetMin(DateTime a, DateTime b)
        {
            return a > b ? b : a;
        }
    }
}