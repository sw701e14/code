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
        private static Database database = new Database();

        /// <summary>
        /// Gets a collection of all bikes and a <see cref="DateTime"/> value indicating when they were "parked".
        /// </summary>
        /// <returns>A collection of bikes and park-times.</returns>
        public static IEnumerable<Tuple<int, DateTime>> GetBikesImmobile()
        {
            Dictionary<int, gps_data> firstData = new Dictionary<int, gps_data>();
            Dictionary<int, gps_data> lastData = new Dictionary<int, gps_data>();
            List<int> stopped = new List<int>();

            foreach (var v in from gps in database.gps_data orderby gps.queried select gps)
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
                    if (gps_data.inVicinity(firstData[v.bikeId], v))
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
        /// <param name="immobileSince">Any bikes that were parked after <paramref name="immobileSince"/> will not be returned.</param>
        /// <returns>A collection of bikes and park-times.</returns>
        public static IEnumerable<Tuple<int, DateTime>> GetBikesImmobile(DateTime immobileSince)
        {
            return GetBikesImmobile().Where(b => b.Item2 < immobileSince);
        }
    }
}
