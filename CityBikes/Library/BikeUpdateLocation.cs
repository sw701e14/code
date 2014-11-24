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
        /// <summary>
        /// Inserts new gps_data into database, depending on whether the bike has moved or not
        /// </summary>
        /// <param name="newLocation">The updated location to be inserted</param>
        public static void InsertLocation(Database.DatabaseSession session, GPSData newLocation)
        {
            var latestLocation = newLocation.Bike.LatestGPSData(session);

            if (GPSData.WithinAccuracy(newLocation, latestLocation))
                session.Execute("UPDATE gps_data SET hasNotMoved=true WHERE bikeId={0}", newLocation.Bike.Id);
            else
                session.Execute("INSERT INTO gps_data (bikeId, latitude, longitude, accuracy, queried, hasNotMoved) VALUES{0}", formatGPS(newLocation));
        }

        private static string formatGPS(GPSData data)
        {
            return string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                data.Bike.Id,
                data.Location.Latitude,
                data.Location.Longitude,
                data.Accuracy,
                data.QueryTime.ToString("yyyy-MM-dd hh:mm:ss"),
                data.HasNotMoved ? '1' : '0');
        }
    }
}
