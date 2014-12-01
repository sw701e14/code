using System;
using System.Collections.Generic;

namespace DataLoading.Common
{
    public static class GPSInputExtension
    {
        private static Random r = new Random();

        public static IEnumerable<GPSInput> ConvertToInterval(this IEnumerable<GPSInput> route, TimeSpan interval)
        {
            if (route == null)
                throw new ArgumentNullException("route");

            if (interval.TotalSeconds <= 0.0)
                throw new ArgumentOutOfRangeException("interval", "The interval used for conversion must be strictly positive.");

            var e = route.GetEnumerator();

            if (!e.MoveNext())
            {
                e.Dispose();
                yield break;
            }

            uint bike = e.Current.BikeId;

            var lastPoint = e.Current;
            yield return lastPoint;
            DateTime nextTime = lastPoint.Timestamp.Add(interval).addRandomSeconds(60);

            while (e.MoveNext())
            {
                var nextPoint = e.Current;

                while (nextPoint.Timestamp >= nextTime)
                {
                    var point = generateBetweenPoint(lastPoint, nextPoint, nextTime);
                    yield return new GPSInput(bike, point.Item1, point.Item2, null, nextTime);
                    nextTime = nextTime.Add(interval).addRandomSeconds(60);
                }

                lastPoint = nextPoint;
            }

            e.Dispose();
        }

        private static DateTime addRandomSeconds(this DateTime dt, double secondsRange)
        {
            return dt.AddSeconds((r.NextDouble() - 0.5) * secondsRange);
        }
        private static Tuple<decimal, decimal> generateBetweenPoint(GPSInput lp, GPSInput np, DateTime time)
        {
            var diff = (np.Timestamp - lp.Timestamp);

            double triptime = diff.TotalSeconds;
            double pointtime = (time - lp.Timestamp).TotalSeconds;

            if (pointtime != 0)
                return Tuple.Create(
                     (decimal)((double)lp.Latitude + (double)(np.Latitude - lp.Latitude) * (pointtime / triptime)),
                     (decimal)((double)lp.Longitude + (double)(np.Longitude - lp.Longitude) * (pointtime / triptime)));
            else
                return Tuple.Create(lp.Latitude, lp.Longitude);
        }
    }
}
