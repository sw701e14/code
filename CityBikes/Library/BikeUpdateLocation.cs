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
        public static int InsertLocation(gps_data newLocation)
        {
            Database db = new Database();

            gps_data latestLocation = db.gps_data.Where(x => x.id == newLocation.id).OrderBy(x => x.queried).FirstOrDefault();

            if (latestLocation != null && gps_data.WithinAccuracy(newLocation, latestLocation))
                latestLocation.hasNotMoved = true;
            else
                db.gps_data.AddObject(newLocation);

            return db.SaveChanges();
        }
    }
}
