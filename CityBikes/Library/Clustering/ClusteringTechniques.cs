using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library.Clustering
{
    public static class ClusteringTechniques
    {
        public static List<List<CorePoint>> DBSCAN(GPSLocation[] gpsLocations, int minimumPoints, double radius)
        {
            int size = gpsLocations.Count();
            List<CorePoint> corePoints = new List<CorePoint>();
            List<Point> neighborhood = new List<Point>();
            int neighborCount = 0;
            for (int i = 0; i < size; i++)
            {
                Console.WriteLine("Location: " + i);
                int count = 1;
                neighborhood.Clear();
                for (int j = 0; j < size; j++)
                {
                    double dist = GPSLocation.Distance(gpsLocations[i], gpsLocations[j]);
                    if (!gpsLocations[i].Equals(gpsLocations[j]) && dist < radius)
                    {
                        neighborhood.Add(new Point(gpsLocations[j]));
                        neighborCount++;
                        count++;
                    }
                }
                if (count>=minimumPoints)
                {
                    CorePoint cp = new CorePoint(gpsLocations[i], new List<Point>(neighborhood));
                    corePoints.Add(cp);
                }
            }
            return findClusters(corePoints);
        }

        private static List<List<CorePoint>> findClusters(List<CorePoint> corePoints)
        {
            List<List<CorePoint>> clusters = new List<List<CorePoint>>();
            List<CorePoint> tmp = new List<CorePoint>();
            List<CorePoint> corePointsCopy = new List<CorePoint>(corePoints);
            foreach (var cp1 in corePoints)
            {
                if (corePointsCopy.Contains(cp1))
                {
                    tmp.Add(cp1);                
                    foreach (var cp2 in corePoints)
                    {
                        if (!cp1.Equals(cp2) && cp2.Neighborhood.Contains(cp1))
                        {
                            tmp.Add(cp2);
                            corePointsCopy.Remove(cp2);
                        }
                    }
                    clusters.Add(new List<CorePoint>(tmp));
                }
                tmp.Clear();
                corePointsCopy.Clear();
            }
            return clusters;
        }
    }
}
