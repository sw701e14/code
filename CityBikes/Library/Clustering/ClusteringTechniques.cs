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
        public static List<CorePoint> DBSCAN(GPSLocation[] gpsLocations, int minimumPoints, decimal radius)
        {
            int size = gpsLocations.Count();
            List<CorePoint> corePoints = new List<CorePoint>();
            List<Point> neighborhood = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                int count = 0;
                neighborhood.Clear();
                for (int j = 0; j < size; j++)
                {
                    if (!gpsLocations[i].Equals(gpsLocations[j]) && GPSTools.GetDistance(gpsLocations[i], gpsLocations[j]) < radius)
                    {
                        neighborhood.Add(new Point(gpsLocations[j]));
                        count++;
                    }
                }
                if (count>=minimumPoints)
                {
                    CorePoint cp = new CorePoint(gpsLocations[i], neighborhood);
                    corePoints.Add(cp);
                }
            }
            return findClusters(corePoints);
        }

        private static List<CorePoint> findClusters(List<CorePoint> corePoints)
        {
            foreach (var cp1 in corePoints)
            {
                foreach (var cp2 in corePoints)
                {
                    if (!cp1.Equals(cp2) && cp1.Neighborhood.Contains(cp2))
                    {
                        cp1.Neighborhood.AddRange(cp2.Neighborhood);
                        cp1.Neighborhood.Add(cp2);
                        corePoints.Remove(cp2);
                    }
                }
            }
            return corePoints; 
        }
    }
}
