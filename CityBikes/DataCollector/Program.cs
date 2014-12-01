using DataSources;
using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataCollector
{
    class Program
    {
        private static Database database;
        private static bool shouldExit;

        private const int SLEEP_MILLISECONDS = 100;

        private static List<Bike> knownBikes = new List<Bike>();

        private static IDataSource createDataSource()
        {
            Console.Write("Specify number of bikes: ");
            string numberStr = Console.ReadLine();
            int count = int.Parse(numberStr);
            Console.WriteLine("Data will be generated for {0} bikes.", count);
            Console.WriteLine();

            List<Bike> bikes = new List<Bike>();
            for (uint i = 10; i < 10 + count; i++)
                bikes.Add(new Bike(i));

            DateTime start = DateTime.Now.Date.AddDays(-1);

            return new MultiDataSource(bikes.Select(b => new NoFutureDataSource(GoogleDataSource.GetSource(b, start, int.MaxValue))));
        }

        static void Main(string[] args)
        {
            shouldExit = false;

            Console.WriteLine("This application will load GPS Data fom the supplied source.");
            Console.WriteLine("Press Q during the process to exit the application.");
            Console.WriteLine();

            var dataSource = createDataSource();
            if (dataSource == null)
                throw new NullReferenceException("dataSource field must be set to an instance of an object.");

            database = new Database();

            database.RunSession(s => s.Execute("TRUNCATE citybike_test.gps_data; TRUNCATE citybike_test.bikes; TRUNCATE citybike_test.hotspots"));

            knownBikes.AddRange(database.RunSession(session => session.ExecuteRead("SELECT * FROM citybike_test.bikes").Select(row => row.GetBike()).ToArray()));

            Thread t = new Thread(o => runDataLoader(o as IDataSource));
            t.Start(dataSource);
            ConsoleKey key = ConsoleKey.A;
            while (key != ConsoleKey.Q)
                key = Console.ReadKey(true).Key;

            shouldExit = true;
            while (t.IsAlive) { }

            database.Dispose();
        }

        private static void runDataLoader(IDataSource dataSource)
        {
            while (!shouldExit)
            {
                var data = dataSource.GetData();

                if (data.HasValue)
                    InsertIntoDB(data.Value);
                else
                    Thread.Sleep(SLEEP_MILLISECONDS);
            }
        }

        private static void InsertIntoDB(GPSData data)
        {
            if (!knownBikes.Contains(data.Bike))
            {
                knownBikes.Add(data.Bike);
                database.RunSession(session => session.Execute("INSERT INTO citybike_test.bikes (id) VALUES ({0})", data.Bike.Id));
            }

            Console.WriteLine("Bike {0:000} at ({1:0.0000}, {2:0.0000}) at {3}.",
                data.Bike.Id,
                data.Location.Latitude,
                data.Location.Longitude,
                data.QueryTime.ToLongTimeString());
            database.RunSession(session => Library.BikeUpdateLocation.InsertLocation(session, data));
        }
    }
}
