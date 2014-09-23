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
        public static string GetBikesNearby()
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
        
        
    }
}
