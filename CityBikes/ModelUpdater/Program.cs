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
        private static double updateModelEveryMinutes = 5;

        static void Main(string[] args)
        {
            Database database = new Database();

            if (args.Any())
                double.TryParse(args[0], out updateModelEveryMinutes);
            if (updateModelEveryMinutes <= 0)
                updateModelEveryMinutes = 5;

            Timer timer = new Timer(e => updateModel(database), null, 0, (long)TimeSpan.FromMinutes(updateModelEveryMinutes).TotalMilliseconds);
            Console.ReadKey();
        }

        private static void updateModel(Database database)
        {
            GPSLocation[] allGPSLocations = getGPSLocationsFromGPSData(database.RunSession(session => SelectQueries.GetAllGPSData(session)));
            List<Shared.DTO.Hotspot> allHotspots = database.RunSession(session => SelectQueries.GetAllHotspots(session));

            //database.RunSession(session => DeleteQueries.TruncateAll(session));

            List<GPSLocation[]> allClusters = ClusteringTechniques.FindClusters(allGPSLocations, 4, 60);

            //MarkovChain markovChain = new MarkovChain(allClusters.Count);




            printClusters(allClusters);
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

        private static void printClusters(List<GPSLocation[]> allClusters)
        {
            for (int i = 0; i < allClusters.Count; i++)
            {
                for (int j = 0; j < allClusters[i].GetLength(0); j++)
                {
                    Console.Write(allClusters[i][j]);
                }
                Console.WriteLine();
                Console.WriteLine();
            }
        }
    }
}
