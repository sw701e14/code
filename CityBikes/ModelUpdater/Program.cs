using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Shared.DTO;
using Shared.DAL;

namespace ModelUpdater
{
    class Program
    {
        private const double UPDATEMODELEVERYMINUTES = 5;
        private const int MINIMUMPOINTSINCLUSTER = 4;
        private const double RADIUSINCLUSTER = 60;

        static void Main(string[] args)
        {
            double updateModelEveryMinutes = UPDATEMODELEVERYMINUTES;

            if (args.Any())
                double.TryParse(args[0], out updateModelEveryMinutes);
            if (updateModelEveryMinutes <= 0)
                updateModelEveryMinutes = UPDATEMODELEVERYMINUTES;

            Timer timer = new Timer(e => updateModel(), null, 0, (long)TimeSpan.FromMinutes(updateModelEveryMinutes).TotalMilliseconds);

            Console.ReadKey(true);
        }

        private static void updateModel()
        {
            Database database = new Database();

            database.RunSession(session => 
            {
                GPSData[] allStandstillGPSData = GPSData.GetAllHasNotMoved(session);
                if (allStandstillGPSData == null || !allStandstillGPSData.Any())
                    return;

                GPSLocation[] allStandstillGPSLocations = getGPSLocationsFromGPSData(allStandstillGPSData);
                if (allStandstillGPSLocations == null || !allStandstillGPSLocations.Any())
                    return;

                List<GPSLocation[]> allClusters = ClusteringTechniques.FindClusters(allStandstillGPSLocations, MINIMUMPOINTSINCLUSTER, RADIUSINCLUSTER);
                if (allClusters == null || !allClusters.Any())
                    return;

                GPSData[] latestGPSData = Bike.GetLatestData(session);

                truncateOldData(session);
                foreach (var point in latestGPSData)
                    GPSData.InsertInDatabase(session, point);

                List<Hotspot> hotspots = new List<Hotspot>();
                foreach (var cluster in allClusters)
                    hotspots.Add(Hotspot.CreateHotspot(session, cluster));

                MarkovChain.CreateMarkovChain(session, hotspots.ToArray(), allStandstillGPSData);

#if DEBUG
                printMatrix(markovChain);
                saveMatrixToFile(markovChain);
#endif
            });

            database.Dispose();
        }

        private static GPSLocation[] getGPSLocationsFromGPSData(GPSData[] data)
        {
            GPSLocation[] allGPSLocations = new GPSLocation[data.Count()];

            int i = 0;
            foreach (GPSData gpsdata in data)
            {
                allGPSLocations[i] = gpsdata.Location;
                i++;
            }

            return allGPSLocations;
        }

        private static void truncateOldData(DatabaseSession session)
        {
            DeleteQueries.TruncateGPS_data(session);
            DeleteQueries.TruncateHotspots(session);
            DeleteQueries.TruncateMarkov_chains(session);
        }

#if DEBUG
        private static void printMatrix(Matrix markov)
        {
            Console.WriteLine("Markov at " + DateTime.UtcNow.ToString());
            for (int i = 0; i < markov.Height; i++)
            {
                for (int j = 0; j < markov.Width; j++)
                {
                    Console.Write(string.Format("{0} ", markov[i, j]));
                }
                Console.Write(Environment.NewLine);
            }
        }

        private static void saveMatrixToFile(Matrix markov)
        {
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter("c:\\test" + DateTime.UtcNow.Ticks.ToString() + ".txt");

            for (int i = 0; i < markov.Height; i++)
            {
                for (int j = 0; j < markov.Width; j++)
                {
                    streamWriter.Write(string.Format("{0} ", markov[i, j]));
                }
                streamWriter.Write(Environment.NewLine);
            }

            streamWriter.Close();
        }
#endif
    }
}
