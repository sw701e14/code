using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    /// <summary>
    /// Represents a hotspot in the system and provides methods for saving/loading hotspots to/from the database.
    /// </summary>
    public class Hotspot
    {
        private GPSLocation[] dataPoints;

        // Hotspots cannot be created directly - they can only be saved to/read from the database.
        internal Hotspot(GPSLocation[] dataPoints)
        {
            if (dataPoints == null)
                throw new ArgumentNullException("dataPoints");

            if (dataPoints.Length == 0)
                throw new ArgumentException("A hotspot must have at least one point.", "dataPoints");

            this.dataPoints = new GPSLocation[dataPoints.Length];
            dataPoints.CopyTo(this.dataPoints, 0);
        }

        public GPSLocation[] getDataPoints()
        {
            return dataPoints;
        }
    }
}
