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
        private GPSLocation[] dataPoints;

        // Hotspots cannot be created directly - they can only be saved to/read from the database.
        public Hotspot(GPSLocation[] dataPoints)
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

        public static GPSLocation[] applyConvexHull(GPSLocation[] data)
        {
            return GPSLocation.GetConvexHull(data);
        }
    }
}
