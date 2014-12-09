﻿using System;
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

        public static void InsertHotSpot(this DatabaseSession session, GPSLocation[] data)
        {
            Hotspot hotspot = new Hotspot(data);

            MySqlCommand cmd = new MySqlCommand("INSERT INTO hotspots (convex_hull) VALUES(@data)");

            BinaryFormatter formatter = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                formatter.Serialize(ms, data);
                cmd.Parameters.Add("@hotspot", MySqlDbType.Blob).Value = ms.ToArray();
            }
             session.Execute(cmd);
        }

        public static void InsertMarkovMatrix(this DatabaseSession session, Matrix matrix)
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
