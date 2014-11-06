using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class BikesNearby
    {
        /// <summary>
        /// Gets a sorted list, based on distance, of bikes and their location.
        /// </summary>
        /// <param name="context">A database context from which data should be retrieved.</param>
        /// <param name="gpsLocation">The GPS location.</param>
        /// <returns>Returns a list of bike id and their location.</returns>
        public static IEnumerable<Tuple<long, GPSLocation>> GetBikesNearby(this Database context, GPSLocation gpsLocation)
        {
            var bikeList = AllBikesLocation.GetBikeLocations(context).ToList();

            var distances = bikeList.ToDictionary(x => x.Item1, x => x.Item2.DistanceTo(gpsLocation));
            bikeList.Sort((x, y) => distances[x.Item1].CompareTo(distances[y.Item1]));

            foreach (var b in bikeList)
                yield return b;
        }
    }
}
