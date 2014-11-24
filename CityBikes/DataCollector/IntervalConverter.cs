using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    public class IntervalConverter
    {
        private GPSData[] route;

        /// <summary>
        /// Generates real points from the route. Real points are spaced out in a fixed time interval 
        /// </summary>
        /// <param name="nextTime">The next time to create point</param>
        /// <param name="interval">The interval.</param>
        /// <param name="route">The route to generate point from</param>
        /// <param name="lastPoint">The index of the last point.</param>
        /// <param name="nextPoint">The index of the next point.</param>
        /// <returns></returns>
        private static IEnumerable<GPSData> convertToIntervalRoute(DateTime nextTime, int interval, List<GPSData> route, int lastPoint, int nextPoint)
        {
            if (nextPoint < route.Count)
            {
                if (route[nextPoint].QueryTime > nextTime)
                {
                    var point = generateBetweenPoint(lastPoint, nextPoint, nextTime);
                    yield return new GPSData(route.First().Bike, point, null, nextTime, false);

                    foreach (var item in convertToIntervalRoute(nextTime.AddMinutes(interval), interval, route, lastPoint, nextPoint))
                        yield return item;
                }
                else
                {
                    foreach (var item in convertToIntervalRoute(nextTime, interval, route, lastPoint + 1, nextPoint + 1))
                        yield return item;
                }
            }
        }

        private GPSLocation generateBetweenPoint(int lastPoint, int nextPoint, DateTime time)
        {
            GPSData np = route[nextPoint];
            GPSData lp = route[lastPoint];

            var diff = (np.QueryTime - lp.QueryTime);

            double triptime = diff.TotalSeconds;

            var g = (time - route[lastPoint].QueryTime);
            double pointtime = g.TotalSeconds;

            if (pointtime != 0)
                return lp.Location + (np.Location - lp.Location) * (pointtime / triptime);
            else
                return lp.Location;
        }
    }
}
