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
        public static string GetBikesNearby(decimal latitude, decimal longitude)
        {
            citybike_testEntities1 context = new citybike_testEntities1();
            var query = from b in context.gps_data
                        orderby b.bikeId
                        select b;

            foreach (gps_data g in query)
            {
                return g.accuracy.ToString();
            }
            return "abekat";
        }

        private static decimal getDistance(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude)
        {
            return Convert.ToDecimal(Math.Sqrt(Math.Pow(Convert.ToDouble(toLatitude - fromLatitude), 2) + Math.Pow(Convert.ToDouble(toLongitude - fromLongitude), 2)));
        }
        
        
    }
}
