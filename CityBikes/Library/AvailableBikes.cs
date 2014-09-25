using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class AvailableBikes
    {
        public static IEnumerable<int> GetAvailableBikes()
        {
            Database database = new Database();
            IQueryable<int> query = from bike in database.gps_data
                                         select bike.bikeId ;

            foreach (int bikeID in query.Distinct())
            {
                if (/*BikeImmobileFor(bikeID) >= AVAILABLE_AFTER_IMMOBILE_FOR_TIME*/true)
                {
                    yield return bikeID;
                }
            }

        }
    }
}
