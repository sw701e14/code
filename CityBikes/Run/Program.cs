using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.GeneratedDatabaseModel;
using Library.Clustering;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            //For testing the bikesnearby method..
            /*
            foreach (var item in BikesNearby.GetBikesNearby(new GPSLocation(2,-2)))
            {
                Console.WriteLine("bikeID: " + item.Item1 + " Distance to given point: " + item.Item2.Latitude + ", " + item.Item2.Longitude);
            }
            Console.ReadKey();
             */
            
            foreach (var item in ClusteringTechniques.DBSCAN(new GPSLocation[] {new GPSLocation(0,2), new GPSLocation(1,1), new GPSLocation(10,10)}, 1, 2))
            {
                Console.WriteLine("Cluster:");
                Console.WriteLine(item.Location.Latitude);
                Console.WriteLine(item.Location.Longitude);
                foreach (var n in item.Neighborhood)
                {
                    Console.WriteLine("Neighborhood:");
                    Console.WriteLine(n.Location.Latitude);
                    Console.WriteLine(n.Location.Longitude);
                }
            }
            Console.ReadKey();
        }
    }
}
