using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.GeneratedDatabaseModel;

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
            Uri link = GPSPointMapPlotter.PlotAllGPSPointImage();
            //Uri map = GPSPointMapPlotter.PlotAllGPSPointMap();
            Console.ReadKey(true);
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
