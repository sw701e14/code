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
        /// <param name="gpsLocation">The GPS location.</param>
        /// <returns>Returns a list of bike id and their location.</returns>
        public static IEnumerable<Tuple<int, GPSLocation>> GetBikesNearby(GPSLocation gpsLocation)
        {
            Database context = new Database();
            var query = from bike in context.gps_data
                        group bike by bike.bikeId into b
                        let newestLocation = b.Max(x => x.queried)

                        from g in b
                        where g.queried == newestLocation
                        select Tuple.Create(g.bikeId, new GPSLocation(g.latitude, g.longitude));

            var bikeList = query.ToList();
            var distances = bikeList.ToDictionary(x => x.Item1, x => x.Item2.DistanceTo(gpsLocation));

            bikeList.Sort((x, y) => distances[x.Item1].CompareTo(distances[y.Item1]));

            foreach (var b in bikeList)
                yield return b;
        }
    }
}
