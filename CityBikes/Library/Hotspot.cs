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

        private Hotspot(GPSLocation[] dataPoints)
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
        /// <param name="context">The database context.</param>
        /// <param name="convexHull">The convex hull.</param>
        public static void SaveToDatabase(this Database context, GPSLocation[] convexHull)
        {
            hotspot hotspot = new hotspot();
            hotspot.convex_hull = serialize(convexHull);

            context.hotspots.AddObject(hotspot);
            context.SaveChanges();
        }

        /// <summary>
        /// Loads all hotspots from database.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <returns>A list of <typeparamref name="GPSLocation[]"/>.</returns>
        public static List<GPSLocation[]> LoadFromDatabase(this Database context)
        {
            List<GPSLocation[]> hotspots = new List<GPSLocation[]>();

            foreach (hotspot hotspot in context.hotspots)
                hotspots.Add(deserialize(hotspot.convex_hull));

            return hotspots;
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

        private static GPSLocation[] deserialize(byte[] blob)
        {
            MemoryStream stream = new MemoryStream(blob);
            BinaryFormatter formatter = new BinaryFormatter();

            GPSLocation[] convex_hull = (GPSLocation[])formatter.Deserialize(stream);

            stream.Close();

            return convex_hull;
        }
    }
}
