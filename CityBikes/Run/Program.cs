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
    }
}
