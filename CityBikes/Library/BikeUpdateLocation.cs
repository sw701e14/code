using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class BikeUpdateLocation
    {
        /// <summary>
        /// Inserts new gps_data into database, depending on whether the bike has moved or not
        /// </summary>
        /// <param name="newLocation">The updated location to be inserted</param>
        public static void InsertLocation(gps_data newLocation)
        {
            Database db = new Database();

            gps_data latestLocation = db.gps_data
                .Where(x => x.bikeId == newLocation.bikeId)
                .OrderByDescending(x => x.queried)
                .FirstOrDefault();

            if (latestLocation != null && gps_data.WithinAccuracy(newLocation, latestLocation))
                latestLocation.hasNotMoved = true;
            else
                db.gps_data.AddObject(newLocation);

            db.SaveChanges();
        }
    }
}
