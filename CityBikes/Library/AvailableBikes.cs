using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class AvailableBikes
    {
        public const int IMMOBILE_MINUTES = 10;

        /// <summary>
        /// Gets a collection of all the available bikes.
        /// </summary>
        /// <returns>A collection of bikes and their location</returns>
        public static IEnumerable<Tuple<int, GPSLocation>> GetAvailableBikes()
        {
            Dictionary<int, GPSLocation> positions = AllBikesLocation.GetBikeLocations().ToDictionary(x => x.Item1, x => x.Item2);
            Dictionary<int, DateTime> immobile = BikeStandstill.GetBikesImmobile().ToDictionary(x => x.Item1, x => x.Item2);

            var immobileTimeSpan = new TimeSpan(0, IMMOBILE_MINUTES, 0);
            DateTime now = DateTime.Now;

            foreach (var pair in immobile)
                if ((now - pair.Value).CompareTo(immobileTimeSpan) > 0)
                    yield return Tuple.Create(pair.Key, positions[pair.Key]);
        }
    }
}
