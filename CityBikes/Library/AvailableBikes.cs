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
        /// <param name="context">A database context from which data should be retrieved.</param>
        /// <returns>A collection of bikes and their location</returns>
        public static IEnumerable<Tuple<long, GPSLocation>> GetAvailableBikes(this Database context)
        {
            Dictionary<long, GPSLocation> positions = AllBikesLocation.GetBikeLocations(context).ToDictionary(x => x.Item1, x => x.Item2);
            Dictionary<long, DateTime> immobile = BikeStandstill.GetBikesImmobile(context).ToDictionary(x => x.Item1, x => x.Item2);

            var immobileTimeSpan = new TimeSpan(0, IMMOBILE_MINUTES, 0);
            DateTime now = DateTime.Now;

            foreach (var pair in immobile)
                if ((now - pair.Value).CompareTo(immobileTimeSpan) > 0)
                    yield return Tuple.Create(pair.Key, positions[pair.Key]);
        }
    }
}
