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

        public Hotspot(GPSLocation[] dataPoints)
        {
            if (dataPoints == null)
                throw new ArgumentNullException("dataPoints");

            if (dataPoints.Length == 0)
                throw new ArgumentException("A hotspot must have at least one point.", "dataPoints");

            this.dataPoints = new GPSLocation[dataPoints.Length];
            dataPoints.CopyTo(this.dataPoints, 0);
        }

        /// <summary>
        /// Takes the resulting <typeparamref name="GPSLocation[]"/> from
        /// <typeparamref name="ConvexHull"/> and saves it to database.
        /// </summary>
        /// <param name="session">The database context.</param>
        /// <param name="data">The convex hull.</param>
        public static Hotspot CreateInDatabase(Database.DatabaseSession session, GPSLocation[] data, bool applyConvexHull)
        {
            if (applyConvexHull)
                data = ConvexHull.GrahamScan(data);

            Hotspot hotspot = new Hotspot(data);
            byte[] buffer = serialize(data);

            MySqlCommand cmd = new MySqlCommand("INSERT INTO hotspots (convex_hull) VALUES(@data)");
            cmd.Parameters.Add("@data", MySqlDbType.Blob).Value = buffer;

            session.Execute(cmd);

            return hotspot;
        }

        private static byte[] serialize(GPSLocation[] convexHull)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();

            formatter.Serialize(stream, convexHull);
            byte[] blob = stream.ToArray();

            stream.Close();

            return blob;
        }
    }
}
