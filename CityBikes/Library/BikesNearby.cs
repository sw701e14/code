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
        public static SortedList<int,Tuple<decimal,DateTime>> GetBikesNearby(decimal latitude, decimal longitude)
        {
            SortedList<int, Tuple<decimal,DateTime>> bikes = new SortedList<int,Tuple<decimal,DateTime>>();
            Database context = new Database();
            var query = from b in context.gps_data
                        select b;

            foreach (gps_data g in query)
            {
                if (bikes.Keys.Contains(g.bikeId))
                {
                    if (DateTime.Compare(bikes[g.bikeId].Item2, g.queried) > 0)
                    {
                        bikes[g.bikeId] = Tuple.Create(getDistance(latitude, longitude, g.latitude, g.longitude), g.queried);
                    }
                }
                else
                {
                    bikes.Add(g.bikeId, Tuple.Create(getDistance(latitude, longitude, g.latitude, g.longitude), g.queried));
                }
                
            }
            return bikes;
        }

        private static decimal getDistance(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude)
        {
            return Convert.ToDecimal(Math.Sqrt(Math.Pow(Convert.ToDouble(toLatitude - fromLatitude), 2) + Math.Pow(Convert.ToDouble(toLongitude - fromLongitude), 2)));
        }
        
    }
}
