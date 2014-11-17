using Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

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
        /// <param name="bike">The bike to which the route should be associated.</param>
        /// <param name="destinations">The destinations.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns></returns>
        public static IEnumerable<GPSData> GenerateBikeRoute(Bike bike, string[] destinations, DateTime startTime, int iterations)
        {
            List<GPSData> result = new List<GPSData>();

            string startPoint = nextDestination(destinations, "");
            string destination = nextDestination(destinations, startPoint);

            Debug.WriteLine("Bike: {0}", bike.Id);
            for (int i = 0; i < iterations; i++)
            {

                Thread.Sleep(500); // max 2 requests pr second in the Google directions API
                Console.WriteLine("Iteration {0}", i);

                var route = GoogleDirectionsParser.GetData(startPoint, destination, startTime, bike).ToList();

                var gpsdata = GenerateRealRoute(startTime, POINTINTERVAL, route);

                foreach (var point in gpsdata)
                {
                    yield return point;
                }

                GPSData lastpoint = route.Last();

                Debug.WriteLine(route.Count() + " points generated");
                Debug.WriteLine("Start: {0}\nEnd: {1}\nStartTime: {2}\nEndTime: {3}", startPoint, destination, startTime, lastpoint.queried);

                startPoint = destination;
                destination = nextDestination(destinations, startPoint);


                startTime = startTime.AddMinutes(POINTINTERVAL * gpsdata.Count());


                foreach (var item in generateBikeStandStill(startTime, POINTINTERVAL))
                {
                    yield return new GPSData(item, lastpoint.latitude, lastpoint.longitude, null, bike);
                    startTime = item;
                }

                startTime = startTime.AddMinutes(POINTINTERVAL);
                Debug.WriteLine("");
            }
        }

        /// <summary>
        /// Generates routes for the specified bikes with the specified array of destinations starting from the specified startTime and iterating the specified number of time
        /// </summary>
        /// <param name="bikes">A collection of bikes to generate routes for.</param>
        /// <param name="destinations">The destinations.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="iterations">The number iterations.</param>
        /// <returns></returns>
        public static IEnumerable<GPSData> GenerateBikeRoutes(IEnumerable<Bike> bikes, string[] destinations, DateTime startTime, int iterations)
        {
            foreach (var bike in bikes)
            {
                Console.WriteLine("Generating bike {0}", bike.Id);
                foreach (var item in GenerateBikeRoute(bike, destinations, startTime, iterations))
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
        public static IEnumerable<GPSData> GenerateRealRoute(DateTime nextTime, int interval, IEnumerable<GPSData> route)
        {
            List<GPSData> point = route.ToList();

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
        private static IEnumerable<GPSData> GenerateRealPoints(DateTime nextTime, int interval, List<GPSData> route, int lastPoint, int nextPoint)
        {
            if (nextPoint < route.Count())
            {
                if (route[nextPoint].queried > nextTime)
                {
                    var point = GenerateBetweenPoint(route, lastPoint, nextPoint, nextTime);
                    yield return new GPSData(nextTime, (decimal)point.Item1, (decimal)point.Item2, null, (int)route.First().Bike);

                    foreach (var item in GenerateRealPoints(nextTime.AddMinutes(interval), interval, route, lastPoint, nextPoint))
                        yield return item;
                }
                else
                {
                    foreach (var item in GenerateRealPoints(nextTime, interval, route, lastPoint + 1, nextPoint + 1))
                        yield return item;
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

            DateTime endTime = startTime + new TimeSpan(0, rand.Next(MAXSTANDSTILLTIME), 0);

            while (startTime < endTime)
            {
                yield return startTime;
                startTime = startTime.AddMinutes(interval);
            }
        }


        /// <summary>
        /// Calculates a point between two points, given a timestamp.
        /// </summary>
        /// <param name="route">The route.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="nextPoint">The next point.</param>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        private static GPSLocation GenerateBetweenPoint(List<GPSData> route, int lastPoint, int nextPoint, DateTime time)
        {
            GPSData np = route[nextPoint];
            GPSData lp = route[lastPoint];

            var diff = (np.QueryTime - lp.QueryTime);

            double triptime = diff.TotalSeconds;

            var g = (time - route[lastPoint].QueryTime);
            double pointtime = g.TotalSeconds;

            double latitude, longitude;

            if (pointtime != 0)
                return lp.Location + (np.Location - lp.Location) * (decimal)(pointtime / triptime);
            else
                return new GPSLocation(0, 0);
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

            do
            {
                dest = destinations[rand.Next(destinations.Length)];
            }
            while (dest == exclude);

            return dest;
        }
    }
}
