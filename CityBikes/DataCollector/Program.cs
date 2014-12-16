using LocationService.Common;
using LocationService.LocationSource;
using DeadDog.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shared.DTO;
using Shared.DAL;

namespace LocationService.DataCollector
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

            double hours = "Hvor mange timer skal vi køre?".GetDouble(x => x > 0);

            return new FixedSpanDataSource( new MultiDataSource(bikes.Select(b => new NoFutureDataSource(GoogleDataSource.GetSource(b.Id, start, int.MaxValue)))),start.AddHours(hours));
        }

        static void Main(string[] args)
        {
            Menu menu = new Menu("Select an option");

            menu.Add("Load data", loadData);
            menu.Add("Clear data", clearData);
            menu.Add("Generate map from DB", () =>
            {
                GPSPointMapPlotter.SaveMapAsHtml();
            });
            menu.Add("Start a server simulation", simulate);
            menu.Add("MAtrixxxxxxxx", matrix);

            menu.SetCancel("Exit");

            menu.Show(true);
        }

        private static void matrix()
        {
            using (Database db = new Database())
            {
                MarkovChain m=  db.RunSession(session => MarkovChain.LoadMarkovChain(session));
                string s = m.Probabilities.ToString();
                System.Diagnostics.Debug.WriteLine(s);
                System.Diagnostics.Debug.WriteLine("");
            }
        }

        static void clearData()
        {
            using (Database db = new Database())
            {
                db.RunSession(session => session.TruncateAll());
            }

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
            DataLoader.StartLoad(createDataSource(), true, true);
        }
    }
}
