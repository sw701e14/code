using Library.GeneratedDatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class BikeStandstill
    {
        /// <summary>
        /// Gets a collection of all bikes and a <see cref="DateTime"/> value indicating when they were "parked".
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>A collection of bikes, last-use-time and a boolean indicating if their are standing still.</returns>
        public static Tuple<Bike, DateTime?, bool>[] GetBikesImmobile(this Database.DatabaseSession session)
        {
            var rows = session.ExecuteRead(
@"SELECT bikes.id, MAX(queried), hasNotMoved
FROM citybike_test.bikes
LEFT OUTER JOIN citybike_test.gps_data
ON citybike_test.bikes.id = citybike_test.gps_data.bikeId
GROUP BY bikes.id");

            return rows.Select(row => row.ToTuple<Bike, DateTime?, bool>()).ToArray();
        }
        /// <summary>
        /// Gets a collection of all bikes and a <see cref="DateTime"/> value indicating when they were "parked".
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <param name="immobileSince">Any bikes that were parked after <paramref name="immobileSince"/> will not be returned.</param>
        /// <returns>A collection of bikes, last-use-time and a boolean indicating if their are standing still.</returns>
        public static Tuple<Bike, DateTime?, bool>[] GetBikesImmobile(this Database.DatabaseSession session, DateTime immobileSince)
        {
            return GetBikesImmobile(session).Where(b => b.Item2 < immobileSince).ToArray();
        }
    }
}
