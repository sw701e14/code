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
                GPSData[] allStandstillGPSData = SelectQueries.GetAllGPSNotMovedData(session);
                if (allStandstillGPSData == null || !allStandstillGPSData.Any())
                    return;

                GPSLocation[] allStandstillGPSLocations = getGPSLocationsFromGPSData(allStandstillGPSData);
                if (allStandstillGPSLocations == null || !allStandstillGPSLocations.Any())
                    return;

                List<GPSLocation[]> allClusters = ClusteringTechniques.FindClusters(allStandstillGPSLocations, MINIMUMPOINTSINCLUSTER, RADIUSINCLUSTER);
                if (allClusters == null || !allClusters.Any())
                    return;

                Hotspot[] allHotspots = convertClustersToHotspots(allClusters);
                if (allHotspots == null || !allHotspots.Any())
                    return;

                Matrix markovChain = MarkovChain.BuildMarkovMatrix(allHotspots, allStandstillGPSData);

                GPSData[] latestGPSData = getAllLatestsGPSData(session);
                truncateOldData(session);
                storeNewData(session, latestGPSData, allHotspots, markovChain);

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

        private static Hotspot[] convertClustersToHotspots(List<GPSLocation[]> allClusters)
        {
            Hotspot[] allHotspots = new Hotspot[allClusters.Count];

            int i = 0;
            foreach (GPSLocation[] item in allClusters)
            {
                Hotspot tempHotspot = new Hotspot(item);
                allHotspots[i] = tempHotspot;
                i++;
            }

            return allHotspots;
        }

        private static GPSData[] getAllLatestsGPSData(DatabaseSession session)
        {
            Bike[] allBikes = SelectQueries.GetBikes(session);
            GPSData[] latestGPSData = new GPSData[allBikes.Length];

            int i = 0;
            foreach (Bike item in allBikes)
            {
                if (SelectQueries.LatestGPSData(session, item) != null)
                {
                    latestGPSData[i] = (GPSData)SelectQueries.LatestGPSData(session, item);
                    i++;
                }
            }

            return latestGPSData;
        }

        private static void truncateOldData(DatabaseSession session)
        {
            DeleteQueries.TruncateGPS_data(session);
            DeleteQueries.TruncateHotspots(session);
            DeleteQueries.TruncateMarkov_chains(session);
        }

        private static void storeNewData(DatabaseSession session, GPSData[] allGPSData, Hotspot[] allHotspots, Matrix markovChain)
        {
            foreach (GPSData item in allGPSData)
	        {
		        InsertQueries.InsertGPSData(session, item);
	        }
            foreach (Hotspot item in allHotspots)
            {
                InsertQueries.InsertHotSpot(session, item.getDataPoints());
            }
            InsertQueries.InsertMarkovMatrix(session, markovChain);
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
