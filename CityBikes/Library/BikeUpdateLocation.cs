using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Library
{
    public static class BikeUpdateLocation
    {
        private static Dictionary<Bike, GPSData> bikesWithKnownLastLocation = new Dictionary<Bike, GPSData>();

        /// <summary>
        /// Inserts new gps_data into database, depending on whether the bike has moved or not
        /// </summary>
        /// <param name="newLocation">The updated location to be inserted</param>
        public static void InsertLocation(Database.DatabaseSession session, GPSData newLocation)
        {
            if (!bikesWithKnownLastLocation.ContainsKey(newLocation.Bike))
            {
                bikesWithKnownLastLocation.Add(newLocation.Bike, newLocation);
                if (!session.ExecuteRead("SELECT bikeId, latitude, longitude, accuracy, queried, hasNotMoved FROM citybike_test.gps_data WHERE bikeId = {0} ORDER BY queried DESC", newLocation.Bike.Id).Any())
                {
                    session.Execute("INSERT INTO gps_data (bikeId, latitude, longitude, accuracy, queried, hasNotMoved) VALUES{0}", formatGPS(newLocation));
                    return;
                }
            }

            var latestLocation = bikesWithKnownLastLocation[newLocation.Bike];

            if (GPSData.WithinAccuracy(newLocation, latestLocation))
                session.Execute(
@"UPDATE gps_data a
INNER JOIN
(
    SELECT  bikeId, MAX(id) id
    FROM    gps_data
    GROUP   BY bikeId
) b ON  a.id = b.id AND {0} = b.bikeId
SET a.hasNotMoved = '1'", newLocation.Bike.Id);
            else
                session.Execute("INSERT INTO gps_data (bikeId, latitude, longitude, accuracy, queried, hasNotMoved) VALUES{0}", formatGPS(newLocation));

            bikesWithKnownLastLocation[newLocation.Bike] = newLocation;
        }

        private static string formatGPS(GPSData data)
        {
            return string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                data.Bike.Id,
                data.Location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                data.Location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                data.Accuracy,
                data.QueryTime.ToString("yyyy-MM-dd HH:mm:ss"),
                data.HasNotMoved ? '1' : '0');
        }
    }
}
