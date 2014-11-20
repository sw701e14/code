using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.GeneratedDatabaseModel;
using Library.Clustering;
using DatabaseImport;
using HtmlAgilityPack;
using System.IO;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            TestClusters();

            Console.ReadKey(true);
            //List<GPSLocation> data = new List<GPSLocation>();
            
            //foreach (var item in from location in context.gps_data select location)
            //{
            //    data.Add(new GPSLocation(item.latitude, item.longitude));
            //}

            //int count = 0;

            //foreach (var item in ClusteringTechniques.DBSCAN(data.Take(5000).ToArray(), 4, 1000))
            //{
            //    Console.WriteLine("Cluster: " + count + " ");
            //    count++;
            //}            
            //Console.ReadKey();
        }

        private static void TestClusters()
        {
            Database b = new Database();

            var locations = (from gps in b.gps_data select gps).ToArray();

            GPSLocation[] gpslocations = locations.Take(1000).Select(x => x.Location).ToArray();

            var clusters = ClusteringTechniques.DBSCAN(gpslocations, 10, 50);


            List<GPSLocation[]> convexClusters = new List<GPSLocation[]>();
            foreach (var cluster in clusters)
            {
                convexClusters.Add(ConvexHull.GrahamScan(cluster));
            }

            foreach (var item in convexClusters)
            {
                Hotspot.SaveToDatabase(b, item);
            }
        }
    }
}
