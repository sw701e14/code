using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTO;
using Shared.DAL;

namespace Webservice.Models
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
                    setHasNotMoved(newLocation.Bike);
            }
            else
            {
                bikesWithKnownLastLocation[newLocation.Bike] = newLocation;
            }
        }

        private static void setHasNotMoved(Bike bike)
        {
            Shared.DAL.UpdateQueries.setHasNotMoved(bike);
            var old = bikesWithKnownLastLocation[bike];
            bikesWithKnownLastLocation[bike] = new GPSData(bike, old.Location, old.Accuracy, old.QueryTime, true);
        }
    }
}
