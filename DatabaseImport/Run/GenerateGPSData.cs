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
        public const int MAXSTANDSTILLTIME = 120;
        public const int POINTINTERVAL = 5;

        public static IEnumerable<GPSPoint> GenerateBikeRoute(int id, string[] destinations, DateTime startTime, int iterations)
        {
            List<GPSPoint> result = new List<GPSPoint>();

            string startPoint = nextDestination(destinations, "");
            string destination = nextDestination(destinations, startPoint);

            for (int i = 0; i < iterations; i++)
            {
                Thread.Sleep(500);
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

        public static IEnumerable<GPSPoint> GenerateRealRoute(DateTime nextTime, int interval, IEnumerable<GPSPoint> route)
        {
            List<GPSPoint> point = route.ToList();

            yield return route.First();

            var g = GenerateRealPoint(nextTime.AddMinutes(interval), interval, point, 0, 1);
            g.ToList();
            foreach (var item in g)
            {
                yield return item;
            }
        }

        private static IEnumerable<GPSPoint> GenerateRealPoint(DateTime nextTime, int interval, List<GPSPoint> route, int lastPoint, int nextPoint)
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

        private static DateTime generateBikeStandStill(DateTime startTime)
        {
            Random rand = new Random();

            return startTime + new TimeSpan(0, rand.Next(MAXSTANDSTILLTIME), 0);

        }


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
