using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class GPSTools
    {
        /// <summary>
        /// Gets the direct distance from one gps location to another. Does not take the globes bearing into account.
        /// </summary>
        /// <param name="fromLatitude">From latitude.</param>
        /// <param name="fromLongitude">From longitude.</param>
        /// <param name="toLatitude">To latitude.</param>
        /// <param name="toLongitude">To longitude.</param>
        /// <returns></returns>
        public static decimal GetDistance(decimal fromLatitude, decimal fromLongitude, decimal toLatitude, decimal toLongitude)
        {
            return Convert.ToDecimal(Math.Sqrt(Math.Pow(Convert.ToDouble(toLatitude - fromLatitude), 2) + Math.Pow(Convert.ToDouble(toLongitude - fromLongitude), 2)));
        }


        /// <summary>
        /// Gets the direct distance from one gps location to another. Does not take the globes bearing into account.
        /// </summary>
        /// <param name="from">From GPSLocation.</param>
        /// <param name="to">To GPSLocation.</param>
        /// <returns></returns>
        public static decimal GetDistance(GPSLocation from, GPSLocation to)
        {
           return Convert.ToDecimal(Math.Sqrt(Math.Pow(Convert.ToDouble(to.Latitude - from.Latitude), 2) + Math.Pow(Convert.ToDouble(to.Longitude - from.Longitude), 2)));
        }
    }
}
