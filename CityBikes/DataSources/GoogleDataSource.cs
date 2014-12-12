using LocationService.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LocationService.LocationSource
{
    public static class GoogleDataSource
    {
        private static readonly TimeSpan interval = TimeSpan.FromMinutes(5);
        private const int MAX_WAIT_MINUTES = 60;

        #region Addresses

        

        private static string[] bycykelstations =
        {
            "Karolinelund, 9000 Aalborg",
            "Strandvejen, 9000 Aalborg",
            "Havnefronten, 9000 Aalborg",
            "Vestbyen st, 9000 Aalborg",
            "Utzon Centret, 9000 Aalborg",
            "Nytorv, 9000 Aalborg",
            "Algade, 9000 Aalborg",
            "Gammeltorv, 9000 Aalborg",
            "Aalborg Zoo, 9000 Aalborg",
            "Fibigerstræde, 9220 Aalborg",
            "Kjellerups torv, 9000 Aalborg",
            "Friis, 9000 Aalborg",
            "Aalborg hallen, 9000 Aalborg",
            "Aalborg st, 9000 Aalborg",
            "Kunsten, 9000 Aalborg",
            "Haraldslund, 9000 Aalborg",
            "Nørresundby Torv, 9400 Nørresundby",
            "Vestergade, 9400 Nørresundby"
        };

        private static string[] otherAddresses =
        {
            "Borgmester Jørgensensvej 5, 9000 Aalborg",
            "Selma Lagerlöfsvej 300, 9220 Aalborg",
            "Sønderbro 25, 9000 Aalborg",
            "Langesgade 16, 9000 Aalborg",
            "Kayerødsgade 10, 9000 Aalborg",
            "Toldstrupsgade 14, 9000 Aalborg",
            "Danmarksgade 30, 9000 Aalborg",
            "Christiansgade 44, 9000 Aalborg",
            "Sankelmarksgade 33, 9000 Aalborg",
            "Vesterbro 30, 9000 Aalborg",
            "Prinsensgade 4, 9000 Aalborg"
        };

        private static string[] allAdresses
        {
            get { return bycykelstations.Concat(otherAddresses).ToArray(); }
        }


        #endregion

        private static Random r = new Random();

        /// <summary>
        /// Generates a route for the specified bike id using a predefined collection of destinations starting from the specified startTime and iterating the specified number of time
        /// </summary>
        /// <param name="bike">The bike to which the route should be associated.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>A <see cref="IDataSource"/> from which the location of the bike can be extracted continuosly.</returns>
        public static IDataSource GetSource(uint bike, DateTime startTime, int iterations)
        {
            return GetSource(bike, allAdresses, startTime, iterations,null);
        }

        /// <summary>
        /// Generates a route for the specified bike id with the specified array of destinations starting from the specified startTime and iterating the specified number of time
        /// </summary>
        /// <param name="bike">The bike to which the route should be associated.</param>
        /// <param name="destinations">The destinations.</param>
        /// <param name="startTime">The start time.</param>
        /// <param name="iterations">The iterations.</param>
        /// <returns>A <see cref="IDataSource"/> from which the location of the bike can be extracted continuosly.</returns>
        public static IDataSource GetSource(uint bike, string[] destinations, DateTime startTime, int iterations, int[] distribution)
        {
            IEnumerable<GPSInput> enumeration = generateBikeRoute(bike, destinations, startTime, iterations,distribution);

            return new EnumerationDataSource(enumeration.ConvertToInterval(interval).Select(s => { s.AddNoise(); return s; }));
        }

        private static IEnumerable<GPSInput> generateBikeRoute(uint bike, string[] destinations, DateTime startTime, int iterations, int[] distribution = null)
        {
            string startPoint = nextDestination(destinations, "",distribution);
            string destination = nextDestination(destinations, startPoint,distribution);

            Debug.WriteLine("Bike: {0}", bike);
            for (int i = 0; i < iterations; i++)
            {
                var route = GoogleDirectionsParser.GetData(startPoint, destination, startTime, bike).ToList();

                foreach (var p in route)
                    yield return p;

                startTime = route.Last().Timestamp.AddMinutes(r.Next(MAX_WAIT_MINUTES));

                startPoint = destination;
                destination = nextDestination(destinations, startPoint,distribution);
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
            {
                dest = destinations[r.Next(destinations.Length)];
            }

            return dest;
        }

        private static string nextDestination(string[] destinations, string exclude, int[] distributions=null)
        {
            if (destinations.Length == 1)
                throw new InvalidOperationException("destinations must contain more than one string");

            string dest = exclude;

            while (dest == exclude)
            {
                dest = chooseDestinationWithDistribution(destinations, distributions, r.Next(100));
            }

            return dest;

        }

        private static string chooseDestinationWithDistribution(string[] dest, int[] dist, int r)
        {
            int p = 0;

            for (int i = 0; i < dest.Length; i++)
            {
                if (r < dist[i] + p)
                    return dest[i];
                else p += dist[i];
            }
            throw new ArgumentException("Not a valid distribution");
        }
    }
}
