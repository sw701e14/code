using Library;
using System;
using System.Threading;

namespace DataCollector
{
    class Program
    {
        private static Database database;
        private static bool shouldExit;

        private static readonly IDataSource dataSource = null;

        private const int SLEEP_MILLISECONDS = 100;

        static void Main(string[] args)
        {
            shouldExit = false;

            if (dataSource == null)
                throw new NullReferenceException("dataSource field must be set to an instance of an object.");

            Console.WriteLine("This application will load GPS Data fom the supplied source.");
            Console.WriteLine("Press Q to exit the application.");

            Console.WriteLine();
            Console.WriteLine("Loading will start in:");
            Console.WriteLine(3);
            Thread.Sleep(1000);
            Console.WriteLine(2);
            Thread.Sleep(1000);
            Console.WriteLine(1);
            Thread.Sleep(1000);

            database = new Database();

            Thread t = new Thread(runDataLoader);
            t.Start();
            Console.ReadKey(true);

            shouldExit = true;
            while (t.IsAlive) { }

            database.Dispose();
        }

        private static void runDataLoader()
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
            Console.WriteLine("Updated the location of bike {0} to ({1}, {2}).", data.Bike.Id, data.Location.Latitude, data.Location.Longitude);
            database.RunSession(session => Library.BikeUpdateLocation.InsertLocation(session, data));
        }
    }
}
