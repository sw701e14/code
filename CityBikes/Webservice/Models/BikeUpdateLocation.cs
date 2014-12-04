using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Shared.DTO;
using Shared.DAL;

namespace Webservice.Model
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
                if (!Shared.DAL.SelectQueries.BikeExists((int)newLocation.Bike.Id))
                {
                    Shared.DAL.InsertQueries.InsertGPSData(newLocation);
                    return;
                }
            }

            var latestLocation = bikesWithKnownLastLocation[newLocation.Bike];

            if (GPSData.WithinAccuracy(newLocation, latestLocation))
            {
                if (!latestLocation.HasNotMoved)
                    setHasNotMoved(session, newLocation.Bike);
            }
            else
            {
                bikesWithKnownLastLocation[newLocation.Bike] = newLocation;
            }
        }

        
        private static void setHasNotMoved(Database.DatabaseSession session, Bike bike)
        {
            session.Execute(
@"UPDATE gps_data a
INNER JOIN
(
    SELECT  bikeId, MAX(id) id
    FROM    gps_data
    GROUP   BY bikeId
) b ON  a.id = b.id AND {0} = b.bikeId
SET a.hasNotMoved = '1'", bike.Id);

            var old = bikesWithKnownLastLocation[bike];
            bikesWithKnownLastLocation[bike] = new GPSData(bike, old.Location, old.Accuracy, old.QueryTime, true);
        }

        
    }
}
