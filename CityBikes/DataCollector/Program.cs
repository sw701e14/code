using DataLoading.Common;
using DataLoading.LocationSource;
using DeadDog.Console;
using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataLoading.DataCollector
{
    class Program
    {
        private static IDataSource createDataSource()
        {
            int count = "Specify number of bikes: ".GetInt32(x => x > 0);

            List<Bike> bikes = new List<Bike>();
            for (uint i = 10; i < 10 + count; i++)
                bikes.Add(new Bike(i));

            DateTime start = DateTime.Now.Date.AddDays(-1);

            return new MultiDataSource(bikes.Select(b => new NoFutureDataSource(GoogleDataSource.GetSource(b.Id, start, int.MaxValue))));
        }

        static void Main(string[] args)
        {
            Menu menu = new Menu("Select an option");

            menu.Add("Load data", loadData);
            menu.Add("Clear data", clearData);
            menu.Add("Generate map from DB", () =>
            {
                using (Database db = new Database())
                    db.RunSession(s => GPSPointMapPlotter.SaveMapAsHtml(s));
            });
            menu.Add("Start a server simulation", simulate);

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
            string from = "From: ".GetString(x => x.Trim().Length > 0);
            string to = "To: ".GetString(x => x.Trim().Length > 0);
            DateTime start = "Start time: ".GetDateTime();
            uint id = (uint)"Specify bike ID: ".GetInt32(x => x >= 0);

            DataLoader.StartLoad(new EnumerationDataSource(GoogleDirectionsParser.GetData(from, to, start, id).Select(x => { x.AddNoise(); return x; })), false, true);
        }
        static void simulate()
        {
            DataLoader.StartLoad(createDataSource(), true, false);
        }
    }
}
