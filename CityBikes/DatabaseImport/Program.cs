﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseImport;
using DeadDog.Console;
using System.IO;
using Library;
using System.Diagnostics;

namespace Run
{
    static class Program
    {
        static void Main(string[] args)
        {
            Menu menu = new Menu("Select an option");

            menu.Add("Load data", loadData);
            menu.Add("Clear data", clearData);

            menu.SetCancel("Exit");

            menu.Show(true);
        }

        static void clearData()
        {
            using (Database database = new Database())
                database.RunSession(session => session.Execute("TRUNCATE TABLE gps_data"));
        }

        static void loadData()
        {
            var points = loadFromGoogle();
            using (Database database = new Database())
                database.RunSession(session => DatabaseExport.Export(session, points));
        }

        static GPSData[] loadFromGoogle()
        {
            string from = "From: ".GetString(x => x.Trim().Length > 0);
            string to = "To: ".GetString(x => x.Trim().Length > 0);
            DateTime start = "Start time: ".GetDateTime();
            uint id = (uint)"Specify bike ID: ".GetInt32(x => x >= 0);

            return GoogleDirectionsParser.GetData(from, to, start, new Bike(id)).ToArray();
        }

        static void insertInDB(IEnumerable<GPSData> points)
        {
            using (var database = new Database())
                database.RunSession(session => DatabaseExport.Export(session, points));
        }
    }
}
