using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class BikesNearby
    {
        public static SortedList<decimal,gps_data> GetBikesNearby(decimal latitude, decimal longitude)
        {
            SortedList<decimal, gps_data> bikes = new SortedList<decimal, gps_data>();
            Database context = new Database();
            var query = from b in context.gps_data
                        select b;

            foreach (gps_data g in query)
            {
                bikes.Add(getDistance(latitude, longitude, g.latitude, g.longitude), g);
            }
            return bikes;
        }

        private static decimal getDistance(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude)
        {
            return Convert.ToDecimal(Math.Sqrt(Math.Pow(Convert.ToDouble(toLatitude - fromLatitude), 2) + Math.Pow(Convert.ToDouble(toLongitude - fromLongitude), 2)));
        }
        
    }
}
