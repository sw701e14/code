using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library.Clustering
{
    public class ClusteringTechniques
    {
        private List<Point> D;

        private ClusteringTechniques(IEnumerable<GPSLocation> data)
        {
            this.D = new List<Point>();

            foreach (var l in data)
                this.D.Add(new Point(l));
        }

        /// <summary>
        /// Finds all clusters from the given locations.
        /// </summary>
        /// <param name="gpsLocations">A collection of all locations.</param>
        /// <param name="minimumPoints">The minimum amount of neighbour points in its vicinity before it can be a core point.</param>
        /// <param name="radius">The radius for a point to be a core point.</param>
        /// <returns>Returns a list of clusters.</returns>
        public static GPSLocation[][] FindClusters(IEnumerable<GPSLocation> gpsLocations, int minimumPoints, double radius)
        {
            ClusteringTechniques ct = new ClusteringTechniques(gpsLocations);
            
            throw new NotImplementedException();
        }

        private List<GPSLocation[]> DBSCAN(int minimumPoints, double radius)
        {
            List<GPSLocation[]> clusters = new List<GPSLocation[]>();

            List<Point> neighbours = new List<Point>();
            int count = 0;
            foreach (var point in D.Where(x => !x.Visited))
            {
                point.Visited = true;
                count++;
                neighbours = findNeighbours(D, point, radius).Select(x => x).ToList();
                if (neighbours.Count >= minimumPoints)
                    clusters.Add(expandCluster(point, neighbours, clusters, minimumPoints, radius));
            }
            return clusters;
        }

        private GPSLocation[] expandCluster(Point point, List<Point> neighbours, List<GPSLocation[]> clusters, int minimumpoints, double radius)
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
