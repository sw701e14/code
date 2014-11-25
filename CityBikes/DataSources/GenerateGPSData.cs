using Library;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DataSources
{
    public static class GenerateGPSData
    {
        /// <summary>
        /// The max time (in minutes) a bike can stand still between routes
        /// </summary>
        private const int MAX_WAIT_MINUTES = 120;

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
            string startPoint = nextDestination(destinations, "");
            string destination = nextDestination(destinations, startPoint);

            Debug.WriteLine("Bike: {0}", bike.Id);
            for (int i = 0; i < iterations; i++)
            {
                Thread.Sleep(200); // max 2 requests pr second in the Google directions API
                Console.WriteLine("Iteration {0}", i);

                var route = GoogleDirectionsParser.GetData(startPoint, destination, startTime, bike).ToList();

                foreach (var p in route)
                    yield return p;

                GPSData lastpoint = route.Last();
                Random r = new Random();
                int random = r.Next(MAX_WAIT_MINUTES);
                yield return new GPSData(bike, lastpoint.Location, null, lastpoint.QueryTime.AddMinutes(random), false);
                startTime = lastpoint.QueryTime.AddMinutes(random);
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
                    yield return item;
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
