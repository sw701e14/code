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
            Dictionary<int, Tuple<double, DateTime, GPSLocation>> bikes = new Dictionary<int, Tuple<double, DateTime, GPSLocation>>();
            Database context = new Database();
            var query =  from bike in context.gps_data
                         group bike by bike.bikeId into b
                         let newestLocation = b.Max(x => x.queried)

                         from g in b
                         where g.queried == newestLocation
                         select g;

            foreach (gps_data g in query)
            {
                bikes.Add(g.bikeId, Tuple.Create(gpsLocation.DistanceTo(g.Location), g.queried, new GPSLocation(g.latitude, g.longitude)));
            }

            foreach (KeyValuePair<int, Tuple<double, DateTime, GPSLocation>> bike in bikes.OrderBy(x => x.Value.Item1))
            {
                yield return Tuple.Create(bike.Key, bike.Value.Item3);
            }
        }

        

    }
}
