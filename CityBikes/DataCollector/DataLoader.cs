using DataLoading.Common;
using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Shared.DAL;
using Shared.DTO;

namespace DataLoading.DataCollector
{
    public class DataLoader
    {
        private Database database;
        private bool shouldExit;

        private IDataSource dataSource;
        private bool prompt, runall;

        private const int SLEEP_MILLISECONDS = 100;

        private List<Bike> knownBikes = new List<Bike>();

        public static void StartLoad(IDataSource dataSource, bool prompt, bool runall)
        {
            DataLoader loader = new DataLoader(dataSource, prompt, runall);

            if (prompt)
            {
                Console.WriteLine("This application will load GPS Data fom the supplied source.");
                Console.WriteLine("Press Q during the process to exit the application.");
                Console.WriteLine();
            }

            Shared.DAL.DeleteQueries.TruncateAll();

            loader.knownBikes.AddRange(Shared.DAL.SelectQueries.GetBikes());

            Thread t = new Thread(o => loader.runDataLoader(o as IDataSource));
            t.Start(dataSource);

            if (!runall)
            {
                ConsoleKey key = ConsoleKey.A;
                while (key != ConsoleKey.Q)
                    key = Console.ReadKey(true).Key;
            }

            loader.shouldExit = true;
            while (t.IsAlive) { }

            loader.database.Dispose();
        }

        private DataLoader(IDataSource dataSource, bool prompt, bool runall)
        {
            if (dataSource == null)
                throw new ArgumentNullException("dataSource");

            this.dataSource = dataSource;
            this.prompt = prompt;
            this.runall = runall;

            shouldExit = false;
        }

        private void runDataLoader(IDataSource dataSource)
        {
            while (!shouldExit)
            {
                var data = dataSource.GetData();

                if (data != null)
                    InsertIntoDB(new GPSData(new Bike(data.BikeId), new GPSLocation(data.Latitude, data.Longitude), data.Accuracy, data.Timestamp));
                else if (runall)
                    break;
                else
                    Thread.Sleep(SLEEP_MILLISECONDS);
            }
        }

        private void InsertIntoDB(GPSData data)
        {
            if (!knownBikes.Contains(data.Bike))
            {
                knownBikes.Add(data.Bike);
                Shared.DAL.InsertQueries.InsertBike(data.Bike.Id);
                
            }

            if (prompt)
                Console.WriteLine("Bike {0:000} at ({1:0.0000}, {2:0.0000}) at {3}.",
                    data.Bike.Id,
                    data.Location.Latitude,
                    data.Location.Longitude,
                    data.QueryTime.ToString("dd/MM HH:mm:ss"));

            database.RunSession(session => Library.BikeUpdateLocation.InsertLocation(session, data));
        }
    }
}
