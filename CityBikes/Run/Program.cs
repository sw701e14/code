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
            //testBikeStandstill();

            int count = 0;
            foreach (var item in ClusteringTechniques.DBSCAN(new GPSLocation[] {new GPSLocation(0,2), new GPSLocation(1,1), new GPSLocation(10,10)}, 1, 2))
            {
                Console.WriteLine("Cluster: " + count + " ");
                count++;                
                foreach (var n in item)
                {
                    Console.WriteLine(n.Location.ToString());
                    foreach (var x in n.Neighborhood)
                    {
                        Console.WriteLine("Neighborhood:");
                        Console.WriteLine(x.Location.ToString());
                    }
                }
            }
            
            Console.ReadKey();
        }

        static void testBikeStandstill()
        {
            Console.WriteLine("Test for GetBikesImmobile with 0 parameters:");
            foreach (Tuple<int, DateTime> bike in BikeStandstill.GetBikesImmobile())
            {
                Console.WriteLine("Bike Id: {0}  Park Time: {1}", bike.Item1, bike.Item2);
            }
            Console.ReadKey(true);

            Console.WriteLine();
            string testDate = "02-09-2014";

            Console.WriteLine("Test for GetBikesImmobile with date (" + testDate + ") parameter:");
            foreach (Tuple<int, DateTime> bike in BikeStandstill.GetBikesImmobile(DateTime.Parse(testDate)))
            {
                Console.WriteLine("Bike Id: {0}  Park Time: {1}", bike.Item1, bike.Item2);
            }
            Console.ReadKey(true);
        }
    }
}
