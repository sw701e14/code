using Library.GeneratedDatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class BikeStandstill
    {
        /// <summary>
        /// Gets a collection of all bikes and a <see cref="DateTime"/> value indicating when they were "parked".
        /// </summary>
        /// <param name="context">A database context from which data should be retrieved.</param>
        /// <returns>A collection of bikes and park-times.</returns>
        public static IEnumerable<Tuple<long, DateTime>> GetBikesImmobile(this Database context)
        {
            Dictionary<long, gps_data> firstData = new Dictionary<long, gps_data>();
            Dictionary<long, gps_data> lastData = new Dictionary<long, gps_data>();
            List<long> stopped = new List<long>();

            foreach (var v in from gps in context.gps_data orderby gps.queried select gps)
            {
                if (stopped.Contains(v.bikeId))
                    continue;

                if (!lastData.ContainsKey(v.bikeId))
                {
                    firstData.Add(v.bikeId, v);
                    lastData.Add(v.bikeId, v);
                }
                else
                {
                    if (gps_data.WithinAccuracy(firstData[v.bikeId], v))
                        lastData[v.bikeId] = v;
                    else
                        stopped.Add(v.bikeId);
                }
            }

            foreach (var v in lastData)
                yield return Tuple.Create(v.Key, v.Value.queried);
        }
        /// <summary>
        /// Gets a collection of all bikes and a <see cref="DateTime"/> value indicating when they were "parked".
        /// </summary>
        /// <param name="context">A database context from which data should be retrieved.</param>
        /// <param name="immobileSince">Any bikes that were parked after <paramref name="immobileSince"/> will not be returned.</param>
        /// <returns>A collection of bikes and park-times.</returns>
        public static IEnumerable<Tuple<long, DateTime>> GetBikesImmobile(this Database context, DateTime immobileSince)
        {
            return GetBikesImmobile(context).Where(b => b.Item2 < immobileSince);
        }
    }
}
