using Shared.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    /// <summary>
    /// Represents a hotspot in the system and provides methods for saving/loading hotspots to/from the database.
    /// </summary>
    public class Hotspot
    {
        private static Dictionary<uint, Hotspot> hotspots;

        /// <summary>
        /// Creates a new hotspot in the database.
        /// The convex hull that makes up the actual hotspot is automatically applied to <paramref name="dataPoints"/>.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> that should be used to insert the hotspot.</param>
        /// <param name="dataPoints">The data points that make up the hotspot. Note that the convex hull is automatically found by this method.</param>
        /// <returns>A new <see cref="Hotspot"/> constructed from <see cref="dataPoints"/></returns>
        public static Hotspot CreateHotspot(DatabaseSession session, GPSLocation[] dataPoints)
        {
            dataPoints = GPSLocation.GetConvexHull(dataPoints);

            uint id = session.InsertHotSpot(dataPoints.Select(x => Tuple.Create(x.Latitude, x.Longitude)).ToArray());
            Hotspot hs = new Hotspot(dataPoints);

            hotspots.Add(id, hs);

            return hs;
        }

        private readonly GPSLocation[] dataPoints;

        private Hotspot(GPSLocation[] dataPoints)
        {
            if (dataPoints == null)
                throw new ArgumentNullException("dataPoints");

            if (dataPoints.Length == 0)
                throw new ArgumentException("A hotspot must have at least one point.", "dataPoints");

            this.dataPoints = new GPSLocation[dataPoints.Length];
            dataPoints.CopyTo(this.dataPoints, 0);
        }

        public bool Contains(GPSLocation location)
        {
            //inspired by http://stackoverflow.com/a/14998816
            bool result = false;
            int j = dataPoints.Length - 1;
            for (int i = 0; i < dataPoints.Length; i++)
            {
                if (dataPoints[i].Latitude < location.Latitude && dataPoints[j].Latitude >= location.Latitude || dataPoints[j].Latitude < location.Latitude && dataPoints[i].Latitude >= location.Latitude)
                {
                    if (dataPoints[i].Longitude + (location.Latitude - dataPoints[i].Latitude) / (dataPoints[j].Latitude - dataPoints[i].Latitude) * (dataPoints[j].Longitude - dataPoints[i].Longitude) < location.Longitude)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
        public bool Contains(GPSData data)
        {
            return Contains(data.Location);
        }

        public double DistanceTo(GPSLocation location)
        {
            double dist = double.MaxValue;

            for (int i = 0; i < dataPoints.Length; i++)
            {
                var d = dataPoints[i].DistanceTo(location);
                if (d < dist) dist = d;
            }

            return dist;
        }
        public double DistanceTo(GPSData data)
        {
            return DistanceTo(data.Location);
        }

        public GPSLocation[] getDataPoints()
        {
            return dataPoints;
        }
    }
}
