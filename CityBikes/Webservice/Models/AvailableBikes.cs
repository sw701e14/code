using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Model
{
    public static class AvailableBikes
    {
        public const int IMMOBILE_MINUTES = 10;

        /// <summary>
        /// Gets a collection of all the available bikes.
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>A collection of bikes and their location.</returns>
        public static Tuple<Bike, GPSLocation>[] GetAvailableBikes(this Database.DatabaseSession session)
        {
            Dictionary<Bike, GPSLocation> positions = session.GetBikeLocations().ToDictionary(x => x.Item1, x => x.Item2);
            Dictionary<Bike, DateTime> immobile = session.GetBikesImmobile().ToDictionary(x => x.Item1, x => x.Item2);

            var immobileTimeSpan = new TimeSpan(0, IMMOBILE_MINUTES, 0);
            DateTime now = DateTime.Now;

            return (from p in immobile
                    where (now - p.Value).CompareTo(immobileTimeSpan) > 0
                    select Tuple.Create(p.Key, positions[p.Key])).ToArray();
        }
    }
}
