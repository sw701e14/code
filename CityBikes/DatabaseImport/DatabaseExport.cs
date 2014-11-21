using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static void Export(Database.DatabaseSession session, IEnumerable<GPSData> points)
        {
            var pArr = points.ToArray();

            List<Bike> oldbikes = session.ExecuteRead("SELECT * FROM citybike_test.bikes").Select(row => row.GetBike()).ToList();
            List<Bike> newbikes = new List<Bike>();

            foreach (var p in pArr)
            {
                if (!oldbikes.Contains(p.Bike))
                {
                    newbikes.Add(p.Bike);
                    oldbikes.Add(p.Bike);
                }

                GPSData movedPoint = MoveRandom(p);
                BikeUpdateLocation.InsertLocation(session, movedPoint);
            }
        }


        static Random r = new Random();
        public static GPSData MoveRandom(GPSData point)
        {
            double angle = r.NextDouble() * 2 * Math.PI;
            double distance = r.Next(20);

            return new GPSData(point.Bike, GPSLocation.Move(point.Location, angle, distance / 1000), point.Accuracy, point.QueryTime, point.HasNotMoved);
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
        private static IEnumerable<GPSData> ConvertToIntervalRoute(DateTime nextTime, int interval, List<GPSData> route, int lastPoint, int nextPoint)
        {
            if (nextPoint < route.Count)
            {
                if (route[nextPoint].QueryTime > nextTime)
                {
                    var point = GenerateBetweenPoint(route, lastPoint, nextPoint, nextTime);
                    yield return new GPSData(route.First().Bike, point, null, nextTime, false);

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

        private static GPSLocation GenerateBetweenPoint(List<GPSData> route, int lastPoint, int nextPoint, DateTime time)
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
