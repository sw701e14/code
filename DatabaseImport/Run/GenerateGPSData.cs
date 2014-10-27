using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatabaseImport
{


    public static class GenerateGPSData
    {
        /// <summary>
        /// The max time a bike can stand still in the generated data
        /// </summary>
        public const int MAXSTANDSTILLTIME = 120;
        /// <summary>
        /// The timeinterval between points
        /// </summary>
        public const int POINTINTERVAL = 5;

        /// <summary>
        /// Generates a route for the specified bike id with the specified array of destinations starting from the specified startTime and iterating the specified number of time
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="destinations">The destinations.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns></returns>
        public static IEnumerable<GPSPoint> GenerateBikeRoute(int id, string[] destinations, DateTime startTime, int iterations)
        {
            List<GPSPoint> result = new List<GPSPoint>();

            string startPoint = nextDestination(destinations, "");
            string destination = nextDestination(destinations, startPoint);

            for (int i = 0; i < iterations; i++)
            {

                Thread.Sleep(500); // max 2 requests pr second in the Google directions API
                Console.WriteLine("Iteration {0}", i);

                var route = GoogleDirectionsParser.GetData(startPoint, destination, startTime, id).ToList();

                var gpsdata = GenerateRealRoute(startTime, POINTINTERVAL, route);
                gpsdata.ToList();
                foreach (var point in route)
                {
                    yield return point;
                }

                GPSPoint lastpoint = route.Last();

                Console.WriteLine(route.Count() + " points generated");
                Console.WriteLine("Start: {0}\nEnd: {1}\nStartTime: {2}\nEndTime: {3}", startPoint, destination, startTime, lastpoint.TimeStamp);

                startPoint = destination;
                destination = nextDestination(destinations, startPoint);

                startTime = generateBikeStandStill(lastpoint.TimeStamp);

                Console.WriteLine();
            }
        }

        /// <summary>
        /// Generates routes for the specified number of bikes with the specified array of destinations starting from the specified startTime and iterating the specified number of time
        /// </summary>
        /// <param name="bikes">The number bikes to generate routes for.</param>
        /// <param name="destinations">The destinations.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="iterations">The number iterations.</param>
        /// <returns></returns>
        public static IEnumerable<GPSPoint> GenerateBikeRoutes(int bikes, string[] destinations, DateTime startTime, int iterations)
        {
            for (int i = 0; i < bikes; i++)
            {
                var route = GenerateBikeRoute(i, destinations, startTime, iterations);
                route.ToList();

                foreach (var item in route)
                {
                    yield return item;
                }
            }

        }

        /// <summary>
        /// Generates a route with real points from the route. Real points are spaced out in a fixed time interval 
        /// </summary>
        /// <param name="nextTime">The next time.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="route">The route.</param>
        /// <returns></returns>
        public static IEnumerable<GPSPoint> GenerateRealRoute(DateTime nextTime, int interval, IEnumerable<GPSPoint> route)
        {
            List<GPSPoint> point = route.ToList();

            yield return route.First();

            foreach (var item in GenerateRealPoints(nextTime.AddMinutes(interval), interval, point, 0, 1))
            {
                yield return item;
            }
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
        private static IEnumerable<GPSPoint> GenerateRealPoints(DateTime nextTime, int interval, List<GPSPoint> route, int lastPoint, int nextPoint)
        {
            if (nextPoint < route.Count())
            {

                if (route[nextPoint].TimeStamp > nextTime)
                {
                    var point = GenerateBetweenPoint(route, lastPoint, nextPoint, nextTime);
                    yield return new GPSPoint(nextTime, point.Item1, point.Item2, 10, 1);
                    foreach (var item in GenerateRealPoint(nextTime.AddMinutes(interval), interval, route, lastPoint + 1, nextPoint + 1))
                    {
                        yield return item;
                    }
                }
                else
                {
                    foreach (var item in GenerateRealPoint(nextTime, interval, route, lastPoint + 1, nextPoint + 1))
                    {
                        yield return item;
                    }

                }
            }
        }

        /// <summary>
        /// Generates a random amount of datetimes from the startTime of the specified interval
        /// </summary>
        /// <param name="startTime">The start time.</param>
        /// <param name="interval">The interval.</param>
        /// <returns>A set of datetimes</returns>
        private static IEnumerable<DateTime> generateBikeStandStill(DateTime startTime, int interval)
        {
            Random rand = new Random();

            return startTime + new TimeSpan(0, rand.Next(MAXSTANDSTILLTIME), 0);

        }


        /// <summary>
        /// Generates the a point between the two points at 
        /// </summary>
        /// <param name="route">The route.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="nextPoint">The next point.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        private static Tuple<double, double> GenerateBetweenPoint(List<GPSPoint> route, int lastPoint, int nextPoint, DateTime time)
        {
            GPSPoint np = route[nextPoint];
            GPSPoint lp = route[lastPoint];

            double triptime = (np.TimeStamp - lp.TimeStamp).Seconds;
            double pointtime = (time - route[lastPoint].TimeStamp).Seconds;

            double part = triptime / pointtime;

            double latitude = (np.Latitude - lp.Latitude) * part;
            double longitude = (np.Longitude - lp.Longitude) * part;

            return new Tuple<double, double>(lp.Latitude + latitude, lp.Longitude + longitude);
        }


        /// <summary>
        /// Returns a random destination from the destinations array that is not the exclude string
        /// </summary>
        /// <param name="destinations">An array of destinastions</param>
        /// <param name="exclude">The string to exclude</param>
        /// <returns>The next destination</returns>
        private static string nextDestination(string[] destinations, string exclude)
        {
            if (destinations.Length == 1)
                throw new InvalidOperationException("destinations must contain more than one string");

            Random rand = new Random();

            string dest;
            while ((dest = destinations[rand.Next(destinations.Length)]) == exclude)
            {
                // when random string is equal to the exclude string, generate new random
            }

            return dest;
        }
    }
}
