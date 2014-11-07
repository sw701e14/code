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
            testGPSPointMapPlotter();

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


            HtmlDocument htmlDocumentAll = GPSPointMapPlotter.PlotAllGPSPointsToMap(context, apiKey, centerLatitude, centerLongtitude, zoom, mapSizeWidth, mapSizeHeight);
            htmlDocumentAll.Save("C:\\htmlFileAll.html");

            HtmlDocument htmlDocumentSelected = GPSPointMapPlotter.PlotSelectedGPSPointsToMap(context, locationList, apiKey, centerLatitude, centerLongtitude, zoom, mapSizeWidth, mapSizeHeight);
            htmlDocumentSelected.Save("C:\\htmlFileSelected.html");

            Console.WriteLine(htmlDocumentAll.DocumentNode.WriteTo());
            Console.WriteLine(htmlDocumentSelected.DocumentNode.WriteTo());
        }
    }
}
