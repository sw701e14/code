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
        public static GPSLocation[,] DBSCAN(int minimumPoints, double radius)
        {

            return null;
        }

        private static List<CorePoint> getCorePoints(GPSLocation[] gpsLocations, decimal[,] distanceMatrix, int minimumPoints, decimal radius)
        {
            List<CorePoint> corePoints = new List<CorePoint>();
            List<Point> neighborhood = new List<Point>();
            for (int i = 0; i < distanceMatrix.GetLength(i); i++)
            {
                int count = 0;
                neighborhood.Clear();
                for (int j = 0; j < distanceMatrix.GetLength(j); j++)
                {                    
                    if (distanceMatrix[i, j] < radius)
                    {
                        neighborhood.Add(new Point(gpsLocations[j]));
                        count++;
                    }
                }
                if (count>minimumPoints)
                {
                    CorePoint cp = new CorePoint(gpsLocations[i], neighborhood);
                    corePoints.Add(cp);
                }
            }
            return corePoints;
        }

        private static List<GPSLocation> findClusters(List<CorePoint> corePoints)
        { return null; }

        private static decimal[,] clalculateDistanceMatrix(GPSLocation[] gpsLocations)
        {
            int size = gpsLocations.Count();
            decimal[,] distanceMatrix = new decimal[size,size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    distanceMatrix[i, j] = GPSTools.GetDistance(gpsLocations[i], gpsLocations[j]);
            return distanceMatrix;
        }
    }
}
