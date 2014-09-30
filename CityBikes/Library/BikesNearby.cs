﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class BikesNearby
    {

        /// <summary>
        /// Gets a sorted list, based on distance, of bikes and their location.
        /// </summary>
        /// <param name="gpsLocation">The GPS location.</param>
        /// <returns>Returns a list of bike id and their location.</returns>
        public static IEnumerable<Tuple<int, GPSLocation>> GetBikesNearby(GPSLocation gpsLocation)
        {
            Dictionary<int, Tuple<decimal, DateTime, GPSLocation>> bikes = new Dictionary<int, Tuple<decimal, DateTime, GPSLocation>>();
            Database context = new Database();
            var query =  from bike in context.gps_data
                         group bike by bike.bikeId into b
                         let newestLocation = b.Max(x => x.queried)

                         from g in b
                         where g.queried == newestLocation
                         select g;

            foreach (gps_data g in query)
            {
                bikes.Add(g.bikeId, Tuple.Create(getDistance(gpsLocation.Latitude, gpsLocation.Longitude, g.latitude, g.longitude), g.queried, new GPSLocation(g.latitude, g.longitude)));
            }

            foreach (KeyValuePair<int, Tuple<decimal, DateTime, GPSLocation>> bike in bikes.OrderBy(x => x.Value.Item1))
            {
                yield return Tuple.Create(bike.Key, bike.Value.Item3);
            }
        }

        /// <summary>
        /// Gets the direct distance from one gps location to another. Does not take the globes bearing into account.
        /// </summary>
        /// <param name="fromLatitude">From latitude.</param>
        /// <param name="fromLongitude">From longitude.</param>
        /// <param name="toLatitude">To latitude.</param>
        /// <param name="toLongitude">To longitude.</param>
        /// <returns></returns>
        private static decimal getDistance(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude)
        {
            return Convert.ToDecimal(Math.Sqrt(Math.Pow(Convert.ToDouble(toLatitude - fromLatitude), 2) + Math.Pow(Convert.ToDouble(toLongitude - fromLongitude), 2)));
        }

    }
}