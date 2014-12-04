using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTO;


namespace Shared.DAL
{
    public class SelectQueries
    {

        public GPSData LatestGPSData(Bike b)
        {
            return Database.RunCommand(session => session.ExecuteRead("SELECT bikeId, latitude, longitude, accuracy, queried, hasNotMoved FROM citybike_test.gps_data WHERE bikeId = {0} ORDER BY queried DESC", b.Id).First().GetGPSData());
        }

        public byte[] AllMarkovChains(int column)
        {
            var serializedMarkovChain = Database.RunCommand(session=>session.ExecuteRead("SELECT mc FROM markov_chains"));
            byte[] data = serializedMarkovChain.ElementAt(column).GetValue<byte[]>();
            
            return data;
        }

        /// <summary>
        /// Gets the location of the bike with <paramref name="id"/>.
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <param name="id">The id of the bike to find the location of.</param>
        /// <returns>The GPSLocation of the bike with <paramref name="id"/>.</returns>
        public GPSLocation GetBikeLocation(long id)
        {
            var rows = Database.RunCommand(session=>
            session.ExecuteRead(
@"SELECT latitude, longitude
FROM citybike_test.gps_data
WHERE bikeId = {0}
ORDER BY queried desc", id));
            
            return rows.First().GetValue<GPSLocation>();
        }

        /// <summary>
        /// Gets the latest location of all bikes
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>An array of Tuples containing a Bike and its location </returns>
        public Tuple<Bike, GPSLocation>[] GetBikeLocations()
        {

            var rows = Database.RunCommand(session => session.ExecuteRead(
@"SELECT g1.bikeId, latitude, longitude
FROM citybike_test.gps_data g1
INNER JOIN (
    SELECT bikeId, MAX(queried) as MaxDate
    FROM citybike_test.gps_data
    GROUP BY bikeId
) g2 ON g1.bikeId = g2.bikeId AND g1.queried = g2.MaxDate"));

            
            return rows.Select(row => row.ToTuple<Bike, GPSLocation>()).ToArray();
        }

    }
}
