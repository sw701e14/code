using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DAL
{
    public static class SelectQueries
    {
        /// <summary>
        /// Gets the last known GPS data for a single bike.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <param name="id">The id of the bike to find the location of.</param>
        /// <returns>A tuple with [latitude, longitude, accuracy, queried, hasNotMoved] or <c>null</c> if no data exists.</returns>
        public static Tuple<decimal, decimal, byte, DateTime, bool> GetLastGPSData(this DatabaseSession session, uint bikeId)
        {
            var data = session.ExecuteRead(
@"SELECT latitude, longitude, accuracy, queried, hasNotMoved
FROM citybike_test.gps_data
WHERE bikeId = {0}
ORDER BY queried DESC", bikeId).FirstOrDefault();

            if (data == null)
                return null;
            else
                return data.ToTuple<decimal, decimal, byte, DateTime, bool>();
        }
        /// <summary>
        /// Gets the last known location of all bikes.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>An array of tuples with [bikeid, latitude, longitude, accuracy, queried, hasNotMoved]</returns>
        public static Tuple<uint, decimal, decimal, byte, DateTime, bool>[] GetLastGPSData(this DatabaseSession session)
        {
            var rows = session.ExecuteRead(
@"SELECT g1.bikeId, latitude, longitude, accuracy, queried, hasNotMoved
FROM citybike_test.gps_data g1
INNER JOIN (
    SELECT bikeId, MAX(queried) as MaxDate
    FROM citybike_test.gps_data
    GROUP BY bikeId
) g2 ON g1.bikeId = g2.bikeId AND g1.queried = g2.MaxDate");

            return rows.Select(row => row.ToTuple<uint, decimal, decimal, byte, DateTime, bool>()).ToArray();
        }
        /// <summary>
        /// Gets all GPS data in the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>An array of tuples with [bikeid, latitude, longitude, accuracy, queried, hasNotMoved]</returns>
        public static Tuple<uint, decimal, decimal, byte, DateTime, bool>[] GetAllGPSData(this DatabaseSession session)
        {
            var rows = session.ExecuteRead(
@"SELECT latitude, longitude, accuracy, queried, hasNotMoved
FROM citybike_test.gps_data");

            return rows.Select(row => row.ToTuple<uint, decimal, decimal, byte, DateTime, bool>()).ToArray();
        }
        /// <summary>
        /// Gets all GPS data in the database where hasNotMoved = true.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>An array of tuples with [bikeid, latitude, longitude, accuracy, queried, hasNotMoved]</returns>
        public static Tuple<uint, decimal, decimal, byte, DateTime, bool>[] GetAllGPSNotMovedData(this DatabaseSession session)
        {
            var rows = session.ExecuteRead(
@"SELECT latitude, longitude, accuracy, queried, hasNotMoved 
FROM citybike_test.gps_data Where hasNotMoved");

            return rows.Select(row => row.ToTuple<uint, decimal, decimal, byte, DateTime, bool>()).ToArray();
        }

        public static Bike[] GetBikes(this DatabaseSession session)
        {
            return session.ExecuteRead("SELECT * FROM citybike_test.bikes").Select(row => row.GetBike()).ToArray();
        }

        public static List<Hotspot> GetAllHotspots(this DatabaseSession session)
        {
            return session.ExecuteRead("SELECT convex_hull FROM hotspots").Select(row => row.GetHotspot()).ToList();
        }
    }
}
