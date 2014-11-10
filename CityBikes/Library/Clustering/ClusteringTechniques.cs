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
        /// <summary>
        /// Finds all clusters from the given locations.
        /// </summary>
        /// <param name="gpsLocations">An array of all locations.</param>
        /// <param name="minimumPoints">The minimum amount of neighbour points in its vicinity before it can be a core point.</param>
        /// <param name="radius">The radius for a point to be a core point.</param>
        /// <returns>Returns a list of clusters.</returns>
        public static List<GPSLocation[]> DBSCAN(GPSLocation[] gpsLocations, int minimumPoints, double radius)
        {
            List<List<Point>> clusters = new List<List<Point>>();

            List<Point> points = new List<Point>();
            foreach (var loc in gpsLocations)
                points.Add(new Point(loc));

            List<Point> neighbours = new List<Point>();
            int count = 0;
            foreach (var point in points.Where(x => !x.Visited))
            {
                point.Visited = true;
                Console.WriteLine(point.Location.Latitude + "," + point.Location.Longitude + " visited and is no. " + count + ".");
                count++;
                neighbours = findNeighbours(points, point, radius).Select(x => x).ToList();
                if (neighbours.Count < minimumPoints)
                {
                    point.Noise = true;
                }
                else
                    clusters.Add(expandCluster(point, neighbours, clusters, minimumPoints, radius));
            }

            return ConvertToGpsLocation(clusters);
        }

        private static List<GPSLocation[]> ConvertToGpsLocation(List<List<Point>> clusters)
        {
            List<GPSLocation[]> locationClusters = new List<GPSLocation[]>();

            foreach (List<Point> item in clusters)
            {
                locationClusters.Add(item.Select(x => x.Location).ToArray());
            }

            return locationClusters;
        }

        private static List<Point> expandCluster(Point point, List<Point> neighbours, List<List<Point>> clusters, int minimumpoints, double radius)
        {
            List<Point> cluster = new List<Point>();
            cluster.Add(point);
            List<Point> tmpNeighbours = new List<Point>();
            foreach (var p in neighbours)
            {
                if (!p.Visited)
                {
                    p.Visited = true;
                    List<Point> n = findNeighbours(neighbours, p, radius).Select(x => x).ToList();
                    if (n.Count >= radius)
                    {
                        foreach (var item in n)
                            tmpNeighbours.Add(item);
                    }
                }
                if (!hasPoint(clusters, p))
                    cluster.Add(p);
            }
            foreach (var item in tmpNeighbours)
                neighbours.Add(item);

            return cluster;
        }

        private static bool hasPoint(List<List<Point>> clusters, Point p)
        {
            foreach (var c in clusters)
            {
                if (c.Contains(p))
                    return true;
            }
            return false;
        }

        private static IEnumerable<Point> findNeighbours(List<Point> points, Point point, double radius)
        {
            return points.Where(p => p.Location.DistanceTo(point.Location) < radius);
        }
    }
}
