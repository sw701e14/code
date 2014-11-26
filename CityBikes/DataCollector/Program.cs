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
            List<Bike> bikes = new List<Bike>();
            for (uint i = 30; i < 35; i++)
                bikes.Add(new Bike(i));

            return new NoFutureDataSource(GoogleDataSource.GetSource(bikes, DateTime.Now, int.MaxValue));
        }

        static void Main(string[] args)
        {
            shouldExit = false;

            var dataSource = createDataSource();
            if (dataSource == null)
                throw new NullReferenceException("dataSource field must be set to an instance of an object.");

            Console.WriteLine("This application will load GPS Data fom the supplied source.");
            Console.WriteLine("Press Q during the process to exit the application.");
            
            Console.WriteLine("Press any key now to start the process.");
            Console.ReadKey(true);
            Console.WriteLine();

            database = new Database();

            knownBikes.AddRange(database.RunSession(session => session.ExecuteRead("SELECT * FROM citybike_test.bikes").Select(row => row.GetBike()).ToArray()));

            Thread t = new Thread(o => runDataLoader(o as IDataSource));
            t.Start(dataSource);
            Console.ReadKey(true);

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

            Console.WriteLine("Updated the location of bike {0} to ({1:0,0000000}, {2:0,0000000}) at {3}.", data.Bike.Id, data.Location.Latitude, data.Location.Longitude, data.QueryTime.ToLongTimeString());
            database.RunSession(session => Library.BikeUpdateLocation.InsertLocation(session, data));
        }
    }
}
