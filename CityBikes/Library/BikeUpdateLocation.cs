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
            if (newLocation == null)
            {
                throw new ArgumentNullException();
            }
            else if (newLocation.accuracy == null || newLocation.bikeId == null || newLocation.hasNotMoved == null || newLocation.id == null || 
                     newLocation.latitude == null || newLocation.longitude == null || newLocation.queried == null)
            {
                throw new ArgumentException("newLocation must not contain null values.", "Please specify a argument without null values.");
            }

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
