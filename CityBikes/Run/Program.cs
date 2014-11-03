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
            //Console.WriteLine("BikesNearby:");
            foreach (var item in BikesNearby.GetBikesNearby(new GPSLocation(2,-2)))
            {
                Console.WriteLine("bikeID: " + item.Item1 + " Distance to given point: " + item.Item2.Latitude + ", " + item.Item2.Longitude);
            }

            Console.WriteLine("AvailableBikes:");

            foreach (int bikeID in AvailableBikes.GetAvailableBikes())
            {
                Console.WriteLine("bikeID: " + bikeID);
            }

            Console.ReadKey();
             */
            //testBikeStandstill();
            //testAvailableBikes();
        }
    }
}
