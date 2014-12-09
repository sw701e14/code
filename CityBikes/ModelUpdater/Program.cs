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
        private const double UPDATEMODELEVERYMINUTES = 1;

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
            Database database = new Database(); //New database each run or same always?

            GPSData[] allGPSData = database.RunSession(session => SelectQueries.GetAllGPSData(session));
            if (allGPSData == null || !allGPSData.Any())
                return;

            GPSLocation[] allGPSLocations = getGPSLocationsFromGPSData(allGPSData);
            if (allGPSLocations == null || !allGPSLocations.Any())
                return;

            database.RunSession(session => DeleteQueries.TruncateGPS_data(session));

            List<GPSLocation[]> allClusters = ClusteringTechniques.FindClusters(allGPSLocations, 4, 60);
            if (allClusters == null || !allClusters.Any())
                return;

            Hotspot[] allHotspots = convertClustersToHotspots(allClusters);
            if (allHotspots == null || !allHotspots.Any())
                return;

            Matrix markovChain = MarkovChain.BuildMarkovMatrix(allHotspots, allGPSData);

            database.RunSession(session => DeleteQueries.TruncateHotspots(session));
            database.RunSession(session => DeleteQueries.TruncateMarkov_chains(session));

            storeDataInDatabase(database, allGPSData, allHotspots, markovChain);

            //Predict?

            database.Dispose();


            ////////////////////////////////////////////////////////////
            //                      For Testing                       //               
            ////////////////////////////////////////////////////////////
            printMatrix(markovChain);
            saveMatrixToFile(markovChain);
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

        private static void storeDataInDatabase(Database database, GPSData[] allGPSData, Hotspot[] allHotspots, Matrix markovChain)
        {
            foreach (GPSData item in allGPSData)
	        {
                database.RunSession(session => InsertQueries.InsertGPSData(session, item));
	        }
            foreach (Hotspot item in allHotspots)
            {
                database.RunSession(session => InsertQueries.InsertHotSpot(session, item.getDataPoints()));
            }
            database.RunSession(session => InsertQueries.InsertMarkovMatrix(session, markovChain));
        }










        ////////////////////////////////////////////////////////////
        //                      For Testing                       //               
        ////////////////////////////////////////////////////////////

        private static void printMatrix(Matrix markov)
        {
            for (int i = 0; i < markov.Height; i++)
            {
                for (int j = 0; j < markov.Width; j++)
                {
                    Console.Write(string.Format("{0} ", markov[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
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
    }
}
