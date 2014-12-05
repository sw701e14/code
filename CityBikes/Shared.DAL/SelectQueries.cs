using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTO;


namespace Shared.DAL
{
    public static class SelectQueries
    {
        public static GPSData LatestGPSData(this Database.DatabaseSession session, Bike b)
        {
            return  session.ExecuteRead("SELECT bikeId, latitude, longitude, accuracy, queried, hasNotMoved FROM citybike_test.gps_data WHERE bikeId = {0} ORDER BY queried DESC", b.Id).First().GetGPSData();
        }

        public static MarkovChain GetMarkovChain(this Database.DatabaseSession session, int column)
        {
            var serializedMarkovChain =  session.ExecuteRead("SELECT mc FROM markov_chains");
            byte[] data = serializedMarkovChain.ElementAt(column).GetValue<byte[]>();

            return MarkovChain.deserializeMarkovChain(data);
        }

        /// <summary>
        /// Gets the location of the bike with <paramref name="id"/>.
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <param name="id">The id of the bike to find the location of.</param>
        /// <returns>The GPSLocation of the bike with <paramref name="id"/>.</returns>
        public static GPSLocation GetBikeLocation(this Database.DatabaseSession session, long id)
        {
            var rows = session.ExecuteRead(
@"SELECT latitude, longitude
FROM citybike_test.gps_data
WHERE bikeId = {0}
ORDER BY queried desc", id);

            return rows.First().GetValue<GPSLocation>();
        }

        public static GPSData GetBikeGPSData(this Database.DatabaseSession session, long id)
        {
            var rows = session.ExecuteRead(
@"SELECT *
FROM citybike_test.gps_data
WHERE bikeId = {0}
ORDER BY queried desc", id);

            return rows.First().GetValue<GPSData>(1);
        }

        /// <summary>
        /// Gets the latest location of all bikes
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>An array of Tuples containing a Bike and its location </returns>
        public static Tuple<Bike, GPSLocation>[] GetBikeLocations(this Database.DatabaseSession session)
        {

            var rows =  session.ExecuteRead(
@"SELECT g1.bikeId, latitude, longitude
FROM citybike_test.gps_data g1
INNER JOIN (
    SELECT bikeId, MAX(queried) as MaxDate
    FROM citybike_test.gps_data
    GROUP BY bikeId
) g2 ON g1.bikeId = g2.bikeId AND g1.queried = g2.MaxDate");


            return rows.Select(row => row.ToTuple<Bike, GPSLocation>()).ToArray();
        }

        public static List<Hotspot> GetAllHotspots(this Database.DatabaseSession session)
        {
            return  session.ExecuteRead("SELECT convex_hull FROM hotspots").Select(row => row.GetHotspot()).ToList();
        }

        public static bool BikeExists(this Database.DatabaseSession session, int bikeId)
        {
            return  session.ExecuteRead("SELECT bikeId, latitude, longitude, accuracy, queried, hasNotMoved FROM citybike_test.gps_data WHERE bikeId = {0} ORDER BY queried DESC", bikeId).Any();
        }

        /// <summary>
        /// Gets a collection of all bikes and a <see cref="DateTime"/> value indicating when they were "parked".
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>A collection of bikes, last-use-time and a boolean indicating if their are standing still.</returns>
        public static Tuple<Bike, DateTime, bool>[] GetBikesImmobile(this Database.DatabaseSession session)
        {
            var rows =  session.ExecuteRead(
@"SELECT g1.bikeId, queried, hasNotMoved
FROM citybike_test.gps_data g1
INNER JOIN (
    SELECT bikeId, MAX(queried) as MaxDate
    FROM citybike_test.gps_data
    GROUP BY bikeId
) g2 ON g1.bikeId = g2.bikeId AND g1.queried = g2.MaxDate");

            return rows.Select(row => row.ToTuple<Bike, DateTime, bool>()).ToArray();
        }
        /// <summary>
        /// Gets a collection of all bikes and a <see cref="DateTime"/> value indicating when they were "parked".
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <param name="immobileSince">Any bikes that were parked after <paramref name="immobileSince"/> will not be returned.</param>
        /// <returns>A collection of bikes, last-use-time and a boolean indicating if their are standing still.</returns>
        public static Tuple<Bike, DateTime, bool>[] GetBikesImmobile(this Database.DatabaseSession session, DateTime immobileSince)
        {
            return GetBikesImmobile(session).Where(b => b.Item2 < immobileSince).ToArray();
        }

        public static Bike[] GetBikes(this Database.DatabaseSession session)
        {
            return  session.ExecuteRead("SELECT * FROM citybike_test.bikes").Select(row => row.GetBike()).ToArray();
        }

        public static GPSData[] GetAllGPSData(this Database.DatabaseSession session)
        {
            return  session.ExecuteRead("SELECT * FROM citybike_test.gps_data").Select(r => r.GetGPSData(1)).ToArray();
        }

    }
}
