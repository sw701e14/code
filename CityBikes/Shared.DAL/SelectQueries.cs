using System;
using System.Collections.Generic;
using System.IO;
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


        /// <summary>
        /// Gets the id of all the bikes in the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>An array of bike ids.</returns>
        public static uint[] GetBikes(this DatabaseSession session)
        {
            return session.ExecuteRead("SELECT id FROM citybike_test.bikes").Select(row => row.GetValue<uint>()).ToArray();
        }


        /// <summary>
        /// Gets all hotspots in the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>An array in which each element is an array of gps locations and an id representing hotspots.</returns>
        public static Tuple<uint, Tuple<decimal, decimal>[]>[] GetAllHotspots(this DatabaseSession session)
        {
            var rows = session.ExecuteRead("SELECT id, convex_hull FROM hotspots").ToArray();
            Tuple<uint, Tuple<decimal, decimal>[]>[] hotspots = new Tuple<uint, Tuple<decimal, decimal>[]>[rows.Length];

            for (int i = 0; i < rows.Length; i++)
            {
                uint id = rows[i].GetValue<uint>(0);
                using (MemoryStream ms = new MemoryStream(rows[i].GetValue<byte[]>(1)))
                using (BinaryReader reader = new BinaryReader(ms))
                {
                    int len = reader.ReadInt32();
                    var hs = new Tuple<decimal, decimal>[len];
                    hotspots[i] = Tuple.Create(id, hs);
                    for (int j = 0; j < len; j++)
                        hs[j] = Tuple.Create(reader.ReadDecimal(), reader.ReadDecimal());
                }
            }

            return hotspots;
        }

        /// <summary>
        /// Gets a single hotspot from the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <param name="id">The hotspot identifier for the hotspot that should be retrieved.</param>
        /// <returns>An array of tuples [decimal, decimal] representing the gps locations of a hotspot.</returns>
        public static Tuple<decimal, decimal>[] GetHotspot(this DatabaseSession session, uint id)
        {
            var row = session.ExecuteRead("SELECT convex_hull FROM hotspots WHERE id = {0}", id).FirstOrDefault();
            if (row == null)
                return null;

            Tuple<decimal, decimal>[] hotspot;

            using (MemoryStream ms = new MemoryStream(row.GetValue<byte[]>()))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                int len = reader.ReadInt32();
                hotspot = new Tuple<decimal, decimal>[len];
                for (int i = 0; i < len; i++)
                    hotspot[i] = Tuple.Create(reader.ReadDecimal(), reader.ReadDecimal());
            }

            return hotspot;
        }
    }
}
