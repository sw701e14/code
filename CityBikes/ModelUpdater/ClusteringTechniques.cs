using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DAL;
using Shared.DTO;

namespace ModelUpdater
{
    public static class ClusteringTechniques<T> where T : IEquatable<T>
    {
        public static T[][] DBSCAN(T[] D, Func<T, T, bool> eps, int MinPts)
        {
            var points = D.Select(x => new Point(x)).ToArray();
            Func<Point, Point[]> method = (Point p) => points.Where(x => eps(p.Location, x.Location)).ToArray();
            return dbScan(points, method, MinPts);
        }

        // The following three methods are based on the pseudo code from:
        // http://en.wikipedia.org/wiki/DBSCAN#Algorithm

        // The psedo code is included in the methods above the code to which they correspond.
        private static T[][] dbScan(Point[] D, Func<Point, Point[]> eps, int MinPts)
        {
            List<Cluster> clusters = new List<Cluster>();

            //C = 0
            Cluster C = null;
            //for each unvisited point P in dataset D
            foreach (var P in D.Where(p => !p.Visited))
            {
                //mark P as visited
                P.SetIsVisited();
                //NeighborPts = regionQuery(P, eps)
                var NeighborPts = regionQuery(P, eps).ToList();
                //if sizeof(NeighborPts) < MinPts
                if (NeighborPts.Count < MinPts)
                {
                    //mark P as NOISE
                }
                //else
                else
                {
                    //C = next cluster
                    C = new Cluster();
                    clusters.Add(C);
                    //expandCluster(P, NeighborPts, C, eps, MinPts)
                    expandCluster(P, NeighborPts, C, eps, MinPts);
                }
            }

            var res = new T[clusters.Count][];
            for (int i = 0; i < clusters.Count; i++)
            {
                res[i] = clusters[i].ToArray();
            }

            return res;
        }
        private static void expandCluster(Point P, List<Point> NeighborPts, Cluster C, Func<Point, Point[]> eps, int MinPts)
        {
            //add P to cluster C
            C.Add(P);
            P.SetInCluster();
            //for each point P' in NeighborPts
            for (int i = 0; i < NeighborPts.Count; i++)
            {
                var Pp = NeighborPts[i];
                //if P' is not visited
                if (!Pp.Visited)
                {
                    //mark P' as visited
                    Pp.SetIsVisited();
                    //NeighborPts' = regionQuery(P', eps)
                    var NeighborPtsp = regionQuery(Pp, eps);
                    //if sizeof(NeighborPts') >= MinPts
                    if (NeighborPtsp.Length >= MinPts)
                        //NeighborPts = NeighborPts joined with NeighborPts'
                        NeighborPts.AddDistinct(NeighborPtsp);
                }
                //if P' is not yet member of any cluster
                if (!Pp.Clustered)
                //    add P' to cluster C
                {
                    C.Add(Pp);
                }
            }
        }
        private static Point[] regionQuery(Point P, Func<Point, Point[]> eps)
        {
            return eps(P);
        }


        private class Cluster
        {
            private List<Point> points;

            public Cluster()
            {
                this.points = new List<Point>();
            }

            public void Add(Point p)
            {
                this.points.Add(p);
            }

            public T[] ToArray()
            {
                return points.Select(x => x.Location).ToArray();
            }
        }
        private class Point : IEquatable<Point>
        {
            private T t;

            public Point(T t)
            {
                this.t = t;
                this.visited = false;
            }

            public T Location
            {
                get { return t; }
            }

            public override bool Equals(object obj)
            {
                if (obj is Point)
                    return Equals(obj as Point);
                else
                    return false;
            }
            public bool Equals(Point other)
            {
                return ReferenceEquals(this, other);
            }

            public override int GetHashCode()
            {
                return t.GetHashCode();
            }

            private bool visited;
            public bool Visited
            {
                get { return visited; }
            }
            public void SetIsVisited()
            {
                if (this.visited)
                    throw new InvalidOperationException("This point has already been visited.");
                this.visited = true;
            }

            private bool clustered;
            public bool Clustered
            {
                get { return clustered; }
            }
            public void SetInCluster()
            {
                if (this.clustered)
                    throw new InvalidOperationException("This point already belongs to a cluster.");
                this.clustered = true;
            }
        }
    }
}
