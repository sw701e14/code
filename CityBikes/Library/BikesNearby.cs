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
        public static IEnumerable<Tuple<int, decimal>> GetBikesNearby(decimal latitude, decimal longitude)
        {
            Dictionary<int, Tuple<decimal, DateTime, GPSLocation>> bikes = new Dictionary<int, Tuple<decimal, DateTime, GPSLocation>>();
            Database context = new Database();
            var query = from b in context.gps_data
                        select b;

            foreach (gps_data g in query)
            {
                if (bikes.Keys.Contains(g.bikeId))
                {
                    if (DateTime.Compare(bikes[g.bikeId].Item2, g.queried) > 0) //Get newest gps location
                    {
                        bikes[g.bikeId] = Tuple.Create(getDistance(latitude, longitude, g.latitude, g.longitude), g.queried, new GPSLocation(g.latitude, g.longitude));
                    }
                }
                else
                {
                    bikes.Add(g.bikeId, Tuple.Create(getDistance(latitude, longitude, g.latitude, g.longitude), g.queried, new GPSLocation(g.latitude, g.longitude)));
                }
            }

            var bikesNearby = bikes.OrderBy(x => x.Value.Item1).ToList();
            foreach (var bike in bikesNearby)
            {
                yield return Tuple.Create(bike.Key, bike.Value.Item1);
            }
        }

        private static decimal getDistance(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude)
        {
            return Convert.ToDecimal(Math.Sqrt(Math.Pow(Convert.ToDouble(toLatitude - fromLatitude), 2) + Math.Pow(Convert.ToDouble(toLongitude - fromLongitude), 2)));
        }

    }
}
