using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class Hotspot
    {
        public static void SaveToDatabase(this Database context, GPSLocation[] convexHull)
        {
            hotspots hotspot = new hotspots();
            hotspot.convex_hull = serialize(convexHull);

            context.hotspots.AddObject(hotspot);
            context.SaveChanges();
        }

        public static List<GPSLocation[]> LoadFromDatabase(this Database context)
        {
            List<GPSLocation[]> hotspots = new List<GPSLocation[]>();

            foreach (hotspots hotspot in context.hotspots)
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
