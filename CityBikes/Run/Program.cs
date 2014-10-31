using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.GeneratedDatabaseModel;
using HtmlAgilityPack;

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

            //testGPSPointMapPlotter();




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

        static void testGPSPointMapPlotter()
        {
            string apiKey = "AIzaSyDY0kkJiTPVd2U7aTOAwhc9ySH6oHxOIYM";
            string centerLatitude = "57.0338295";
            string centerLongtitude = "9.9277601";
            string zoom = "12";
            string mapSizeWidth = "600";
            string mapSizeHeight = "600";

            Database context = new Database();
            IQueryable<gps_data> locationList = from locations in context.gps_data
                                                where locations.bikeId > 2
                                                select locations;


            Uri link = GPSPointMapPlotter.PlotAllGPSPointsToImage();

            HtmlDocument htmlDocumentAll = GPSPointMapPlotter.PlotAllGPSPointsToMap(apiKey, centerLatitude, centerLongtitude, zoom, mapSizeWidth, mapSizeHeight);
            htmlDocumentAll.Save("C:\\htmlFileAll.html");

            HtmlDocument htmlDocumentSelected = GPSPointMapPlotter.PlotSelectedGPSPointsToMap(locationList, apiKey, centerLatitude, centerLongtitude, zoom, mapSizeWidth, mapSizeHeight);
            htmlDocumentSelected.Save("C:\\htmlFileSelected.html");

            Console.WriteLine(htmlDocumentAll.DocumentNode.WriteTo());
            Console.WriteLine(htmlDocumentSelected.DocumentNode.WriteTo());
        }
    }
}
