using Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataSources
{
    public static class GoogleDataSource
    {
        private static readonly TimeSpan interval = TimeSpan.FromMinutes(5);
        private const int MAX_WAIT_MINUTES = 120;

        private static Random r = new Random();

        /// <summary>
        /// Generates a route for the specified bike id with the specified array of destinations starting from the specified startTime and iterating the specified number of time
        /// </summary>
        /// <param name="bike">The bike to which the route should be associated.</param>
        /// <param name="destinations">The destinations.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>A <see cref="IDataSource"/> from which the location of the bike can be extracted continuosly.</returns>
        public static IDataSource GetSource(Bike bike, string[] destinations, DateTime startTime, int iterations)
        {
            IEnumerable<GPSData> enumeration = generateBikeRoute(bike, destinations, startTime, iterations);

            return new EnumerationDataSource(enumeration.Randomize().ConvertToInterval(interval));
        }

        /// <summary>
        /// Generates routes for the specified bikes with the specified array of destinations starting from the specified startTime and iterating the specified number of time
        /// </summary>
        /// <param name="bikes">A collection of bikes to generate routes for.</param>
        /// <param name="destinations">The destinations.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="iterations">The number iterations.</param>
        /// <returns>A <see cref="IDataSource"/> from which the location of all bikes can be extracted continuosly.</returns>
        public static IDataSource GetSource(IEnumerable<Bike> bikes, string[] destinations, DateTime startTime, int iterations)
        {
            return new MultiDataSource(bikes.Select(b => GetSource(b, destinations, startTime, iterations)));
        }

        private static IEnumerable<GPSData> generateBikeRoute(Bike bike, string[] destinations, DateTime startTime, int iterations)
        {
            string startPoint = nextDestination(destinations, "");
            string destination = nextDestination(destinations, startPoint);

            Debug.WriteLine("Bike: {0}", bike.Id);
            for (int i = 0; i < iterations; i++)
            {
                Thread.Sleep(200); // max 2 requests pr second in the Google directions API

                var route = GoogleDirectionsParser.GetData(startPoint, destination, startTime, bike).ToList();

                foreach (var p in route)
                    yield return p;

                GPSData lastpoint = route.Last();
                int random = r.Next(MAX_WAIT_MINUTES);
                yield return new GPSData(bike, lastpoint.Location, null, lastpoint.QueryTime.AddMinutes(random), false);
                startTime = lastpoint.QueryTime.AddMinutes(random);
            }
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

            string dest = exclude;

            while (dest == exclude)
                dest = destinations[r.Next(destinations.Length)];

            return dest;
        }
    }
}
