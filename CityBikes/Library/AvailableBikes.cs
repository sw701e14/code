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
        /// <summary>
        /// Gets a collection of all the available bikes.
        /// </summary>
        /// <returns>A collection of bikes and their location</returns>
        public static IEnumerable<Tuple<int, GPSLocation>> GetAvailableBikes()
        {
            //Minutes before a bike is considered as available.
            double acceptableStandStillMinutes = 10;

            AllBikesLocation allBikeLocationClass = new AllBikesLocation();

            IEnumerable<Tuple<int, GPSLocation>> allBikesPosition = allBikeLocationClass.GetBikeLocations();
            IEnumerable<Tuple<int, DateTime>> allBikesImmobile = BikeStandstill.GetBikesImmobile();

            var allBikes = allBikesPosition.Join(allBikesImmobile, p => p.Item1, i => i.Item1, (p, i) => new { tuple = Tuple.Create(p.Item1, p.Item2, i.Item2) });

            foreach (var bike in allBikes)
            {
                if (DateTime.Now.Subtract(bike.tuple.Item3).CompareTo(TimeSpan.FromMinutes(acceptableStandStillMinutes)) == 1)
                    yield return Tuple.Create(bike.tuple.Item1, bike.tuple.Item2);
            }
        }
    }
}
