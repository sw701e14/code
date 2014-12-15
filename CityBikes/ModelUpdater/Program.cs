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
        private const double RADIUSINCLUSTER = 15;

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
                if (allStandstillGPSData == null || allStandstillGPSData.Length == 0)
                    return;

                GPSLocation[] allStandstillGPSLocations = getGPSLocationsFromGPSData(allStandstillGPSData);
                if (allStandstillGPSLocations == null || allStandstillGPSLocations.Length == 0)
                    return;

                GPSLocation[][] allClusters = ClusteringTechniques<GPSLocation>.DBSCAN(allStandstillGPSLocations, (a, b) => a.DistanceTo(b) < RADIUSINCLUSTER, MINIMUMPOINTSINCLUSTER);
                if (allClusters == null || allClusters.Length == 0)
                    return;

                GPSData[] latestGPSData = Bike.GetLatestData(session);

                truncateOldData(session);
                foreach (var point in latestGPSData)
                    GPSData.InsertInDatabase(session, point);

                List<Hotspot> hotspots = new List<Hotspot>();
                foreach (var cluster in allClusters)
                {
                    var hs = Hotspot.CreateHotspot(session, cluster);
                    if (hs != null)
                        hotspots.Add(hs);
                }

                GPSData[] data = GPSData.GetAll(session);
                MarkovChain.CreateMarkovChain(session, hotspots.ToArray(), data);
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
    }
}
