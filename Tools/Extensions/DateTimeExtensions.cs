using System;

namespace Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime Round(this DateTime date, long ticks = TimeSpan.TicksPerSecond) {
            if (ticks > 1)
            {
                var frac = date.Ticks % ticks;
                if (frac != 0)
                {
                    if (frac * 2 >= ticks)
                        return new DateTime(date.Ticks + ticks - frac);
                    else
                        return new DateTime(date.Ticks - frac);
                }
            }

            return date;
        }

        public static TimeSpan RoundSeconds(this TimeSpan span, int nDigits)
        {
            return TimeSpan.FromTicks((long) (Math.Round(span.TotalSeconds, nDigits) * TimeSpan.TicksPerSecond));
        }
    }
}