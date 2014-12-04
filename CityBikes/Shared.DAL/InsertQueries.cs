﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Shared.DTO;


namespace Shared.DAL
{
    public static class InsertQueries
    {
        public static void InsertMarkovChain(Database.DatabaseSession session, MarkovChain markovChain)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO markov_chains (mc) VALUES(@data)");
            cmd.Parameters.Add("@data", MySqlDbType.MediumBlob).Value = markovChain.serializeMarkovChain();

            session.Execute(cmd);
        }

        public static void InsertGPSData(GPSData newLocation)
        {
            Database.RunCommand(session=>session.Execute("INSERT INTO gps_data (bikeId, latitude, longitude, accuracy, queried, hasNotMoved) VALUES{0}", formatGPS(newLocation))); 
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
    }
}
