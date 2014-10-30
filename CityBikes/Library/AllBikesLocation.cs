using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class AllBikesLocation
    {
        static Database context = new Database();

        /// <summary>
        /// Gets the location of the bike with <paramref name="Id"/>.
        /// </summary>
        /// <param name="Id">The id of the bike to find the location of.</param>
        /// <returns>The GPSLocation of the bike with <paramref name="Id"/> or null if var location is null</returns>
        public static GPSLocation GetBikeLocation(int Id)
        {
            var location = (from bike in context.gps_data where bike.bikeId == Id orderby bike.queried descending select bike);

            return location.FirstOrDefault().Location;
        }

        /// <summary>
        /// Gets the latest location of all bikes
        /// </summary>
        /// <returns>An IEnumerable containing a Tuple for each bike with its bikeId and its location </returns>
        public static IEnumerable<Tuple<int, GPSLocation>> GetBikeLocations()
        {
            var latest = from bike in context.gps_data
                         group bike by bike.bikeId into b
                         let newestLocation = b.Max(x => x.queried)

                         from g in b
                         where g.queried == newestLocation
                         select g;

            foreach (var item in latest)
            {
                yield return Tuple.Create(item.bikeId, item.Location);
            }
        }
    }
}
