using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models
{
    public static class BikesNearby
    {
        /// <summary>
        /// Gets a sorted list, based on distance, of bikes and their location.
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <param name="gpsLocation">The GPS location from which distance should be measured.</param>
        /// <returns>A list of bikes and their location sorted by their distance to <paramref name="gpsLocation"/>.</returns>
        public static IEnumerable<Tuple<Bike, GPSLocation>> GetBikesNearby(GPSLocation gpsLocation)
        {
            using (Database db = new Database())
            {
                var bikeList = db.RunSession(session=>session.GetBikeLocations().ToList());
            

            var distances = bikeList.ToDictionary(x => x.Item1, x => x.Item2.DistanceTo(gpsLocation));
            bikeList.Sort((x, y) => distances[x.Item1].CompareTo(distances[y.Item1]));

            foreach (var b in bikeList)
                yield return b;
            }
        }
    }
}
