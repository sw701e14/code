using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Shared.DAL
{
    public static class InsertQueries
    {
        /// <summary>
        /// Inserts a new GPS data entity into the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> using which data should be inserted.</param>
        /// <param name="bikeId">The bike identifier.</param>
        /// <param name="latitude">The latitude.</param>
        /// <param name="longitude">The longitude.</param>
        /// <param name="accuracy">The accuracy.</param>
        /// <param name="queryTime">The query time.</param>
        public static void InsertGPSData(this DatabaseSession session, uint bikeId, decimal latitude, decimal longitude, byte accuracy, DateTime queryTime)
        {
            session.Execute("INSERT INTO gps_data (bikeId, latitude, longitude, accuracy, queried, hasNotMoved) VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '0')",
                bikeId,
                latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                longitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                accuracy,
                queryTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        /// <summary>
        /// Inserts a new the bike entity into the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> using which data should be inserted.</param>
        /// <param name="bikeId">The bike identifier.</param>
        public static void InsertBike(this DatabaseSession session, uint bikeId)
        {
            session.Execute("INSERT INTO citybike_test.bikes (id) VALUES ({0})", bikeId);
        }

        /// <summary>
        /// Inserts a new hotspot into the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession" /> using which data should be inserted.</param>
        /// <param name="data">An array of gps location data representing the hotspot.</param>
        /// <returns>The id associated with the hotspot, in the database.</returns>
        public static uint InsertHotSpot(this DatabaseSession session, Tuple<decimal, decimal>[] data)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO hotspots (convex_hull) VALUES(@hotspot)");

            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(data.Length);
                for (int i = 0; i < data.Length; i++)
                {
                    writer.Write(data[i].Item1);
                    writer.Write(data[i].Item2);
                }
                cmd.Parameters.Add("@hotspot", MySqlDbType.Blob).Value = ms.ToArray();
            }
            session.Execute(cmd);

            return session.ExecuteRead("SELECT id FROM hotspots ORDER BY id DESC").First().GetValue<uint>();
        }

        /// <summary>
        /// Inserts a new markov matrix into the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> using which data should be inserted.</param>
        /// <param name="matrix">A two-dimensional matrix of probabilities representing the matrix.</param>
        public static void InsertMarkovMatrix(this DatabaseSession session, double[,] matrix)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(ms))
            {
                int w = matrix.GetLength(0);
                int h = matrix.GetLength(1);

                writer.Write(w);
                writer.Write(h);

                double[,] m = new double[w, h];
                for (int x = 0; x < w; x++)
                    for (int y = 0; y < h; y++)
                        writer.Write(matrix[x, y]);

                MySqlCommand cmd = new MySqlCommand("INSERT INTO markov_chains (mc) VALUES(@data)");
                cmd.Parameters.Add("@data", MySqlDbType.MediumBlob).Value = ms.ToArray();
                session.Execute(cmd);
            }
        }


        public static bool TestDatabase(this DatabaseSession session)
        {
            if (!System.IO.File.Exists("test_data.sql"))
            {
                return false;
            }
            string sqlContent = System.IO.File.ReadAllText("test_data.sql");

            session.Execute(sqlContent);

            return true;
        }
    }
}
