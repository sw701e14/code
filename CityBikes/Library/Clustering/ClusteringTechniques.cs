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
            List<GPSLocation[]> clusters = new List<GPSLocation[]>();

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
                if (neighbours.Count >= minimumPoints)
                    clusters.Add(expandCluster(point, neighbours, clusters, minimumPoints, radius));
            }
            return clusters;
        }

        private static GPSLocation[] expandCluster(Point point, List<Point> neighbours, List<GPSLocation[]> clusters, int minimumpoints, double radius)
        {
            List<GPSLocation> cluster = new List<GPSLocation>();
            cluster.Add(point.Location);
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
                    cluster.Add(p.Location);
            }
            foreach (var item in tmpNeighbours)
                neighbours.Add(item);

            return cluster.ToArray();
        }

        private static bool hasPoint(List<GPSLocation[]> clusters, Point p)
        {
            foreach (var c in clusters)
            {
                if (c.Contains(p.Location))
                    return true;
            }
            return false;
        }

        private static IEnumerable<Point> findNeighbours(List<Point> points, Point point, double radius)
        {
            return points.Where(p => p.Location.DistanceTo(point.Location) < radius);
        }

        private class Cluster
        {
            private List<Point> points;

            public Cluster()
                : this(new Point[0])
            {
            }
            public Cluster(IEnumerable<Point> points)
            {
                this.points = new List<Point>(points);
            }
        }

        private class Point : IEquatable<Point>
        {
            public Point(GPSLocation gpsLocation)
            {
                this.location = gpsLocation;
                this.visited = false;
            }

            private GPSLocation location;

            public GPSLocation Location
            {
                get { return location; }
            }

            public bool Equals(Point other)
            {
                return location.Equals(other.location);
            }

            private bool visited;

            public bool Visited
            {
                get { return visited; }
                set { visited = value; }
            }
        }
    }
}
