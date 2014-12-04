using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Shared.DTO;


namespace Shared.DAL
{
    public static class InsertQueries
    {
        public static void InsertGPSData(this Database.DatabaseSession session, GPSData newLocation)
        {
             session.Execute("INSERT INTO gps_data (bikeId, latitude, longitude, accuracy, queried, hasNotMoved) VALUES{0}", formatGPS(newLocation));
        }

        private static string formatGPS(GPSData data)
        {
            return string.Format("('{0}', '{1}', '{2}', '{3}', '{4}', '{5}')",
                data.Bike.Id,
                data.Location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                data.Location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                data.Accuracy,
                data.QueryTime.ToString("yyyy-MM-dd HH:mm:ss"),
                data.HasNotMoved ? '1' : '0');
        }

        public static void InsertHotSpot(this Database.DatabaseSession session, GPSLocation[] data)
        {
            Hotspot hotspot = new Hotspot(data);

            MySqlCommand cmd = new MySqlCommand("INSERT INTO hotspots (convex_hull) VALUES(@hotspot)");

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, data);
                cmd.Parameters.Add("@data", MySqlDbType.Blob).Value = ms.ToArray();
            }
             session.Execute(cmd);
        }

        public static void InsertMarkovMatrix(this Database.DatabaseSession session, Matrix matrix)
        {
            using(MemoryStream ms = new MemoryStream())
            using(BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(matrix.Width);
                writer.Write(matrix.Height);

                double[,] m = new double[matrix.Width, matrix.Height];
                for (int y = 0; y < matrix.Height; y++)
                    for (int x = 0; x < matrix.Width; x++)
                        writer.Write(matrix[x, y]);

                MySqlCommand cmd = new MySqlCommand("INSERT INTO markov_chains (mc) VALUES(@data)");
                cmd.Parameters.Add("@data", MySqlDbType.MediumBlob).Value = ms.ToArray();
                session.Execute(cmd);
            }
        }

        public static void InsertBike(this Database.DatabaseSession session,uint bikeId)
        {
             session.Execute("INSERT INTO citybike_test.bikes (id) VALUES ({0})", bikeId);
        }
    }
}
