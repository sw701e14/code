using Library.GeneratedDatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace DatabaseImport
{
    public static class DatabaseExport
    {
        private const int INTERVAL = 5;
        /// <summary>
        /// Exports the specified points directly to the database.
        /// </summary>
        /// <param name="context">The database to which the point should be exported.</param>
        /// <param name="points">The points that should be exported.</param>
        public static void Export(Database context, IEnumerable<gps_data> points)
        {
            List<long> bikes = context.bikes.Select(x => x.id).ToList();

            foreach (var p in ConvertToIntervalRoute(points.First().queried,INTERVAL, points.ToList(), 0, 1))
            {
                if(!bikes.Contains(p.bikeId))
                {
                    context.bikes.AddObject(new bike() { id = p.bikeId });
                    bikes.Add(p.bikeId);
                }

                gps_data movedPoint = MoveRandom(p);
                BikeUpdateLocation.InsertLocation(context, movedPoint);
            }
        }


        static Random r = new Random();
        public static gps_data MoveRandom(gps_data point)
        {
            double angle = r.NextDouble() * 2 * Math.PI;
            double distance = r.Next(20);

            return gps_data.Move(point, angle, distance/1000);
        }

        /// <summary>
        /// Generates real points from the route. Real points are spaced out in a fixed time interval 
        /// </summary>
        /// <param name="nextTime">The next time to create point</param>
        /// <param name="interval">The interval.</param>
        /// <param name="route">The route to generate point from</param>
        /// <param name="lastPoint">The index of the last point.</param>
        /// <param name="nextPoint">The index of the next point.</param>
        /// <returns></returns>
        private static IEnumerable<gps_data> ConvertToIntervalRoute(DateTime nextTime, int interval, List<gps_data> route, int lastPoint, int nextPoint)
        {
            if (nextPoint < route.Count)
            {
                if (route[nextPoint].queried > nextTime)
                {
                    var point = GenerateBetweenPoint(route, lastPoint, nextPoint, nextTime);
                    yield return new gps_data(nextTime, (decimal)point.Item1, (decimal)point.Item2, null, (int)route.First().bikeId);

                    foreach (var item in ConvertToIntervalRoute(nextTime.AddMinutes(interval), interval, route, lastPoint, nextPoint))
                        yield return item;
                }
                else
                {
                    foreach (var item in ConvertToIntervalRoute(nextTime, interval, route, lastPoint + 1, nextPoint + 1))
                        yield return item;
                }
            }
        }

        /// <summary>
        /// Generates the a point between the two points at 
        /// </summary>
        /// <param name="route">The route.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="nextPoint">The next point.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        private static Tuple<double, double> GenerateBetweenPoint(List<gps_data> route, int lastPoint, int nextPoint, DateTime time)
        {
            gps_data np = route[nextPoint];
            gps_data lp = route[lastPoint];

            var diff = (np.queried - lp.queried);

            double triptime = diff.TotalSeconds;

            var g = (time - route[lastPoint].queried);
            double pointtime = g.TotalSeconds;

            double latitude, longitude;

            if (pointtime != 0)
            {
                double part = pointtime / triptime;

                latitude = (double)(np.latitude - lp.latitude) * part;
                longitude = (double)(np.longitude - lp.longitude) * part;
            }
            else
            {
                latitude = 0;
                longitude = 0;
            }

            return new Tuple<double, double>((double)lp.latitude + latitude, (double)lp.longitude + longitude);
        }
    }
}
