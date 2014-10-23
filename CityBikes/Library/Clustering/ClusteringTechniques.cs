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

        private static double[,] clalculateDistanceMatrix(GPSLocation[] gpsLocations)
        {
            int size = gpsLocations.Count();
            double[,] distanceMatrix = new double [size,size];
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    //distanceMatrix[i,j] = GPSTools.GetDistance
                }
            }

            return distanceMatrix;
        }
    }
}
