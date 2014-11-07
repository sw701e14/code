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
            Database context = new Database();
            IQueryable<gps_data> locationList = from locations in context.gps_data
                                                where locations.bikeId > 2
                                                select locations;


            HtmlDocument htmlDocumentAll = GPSPointMapPlotter.PlotAllGPSPointsToMap(context);
            htmlDocumentAll.Save("C:\\htmlFileAll.html");

            HtmlDocument htmlDocumentSelected = GPSPointMapPlotter.PlotSelectedGPSPointsToMap(context, locationList);
            htmlDocumentSelected.Save("C:\\htmlFileSelected.html");

            Console.WriteLine(htmlDocumentAll.DocumentNode.WriteTo());
            Console.WriteLine(htmlDocumentSelected.DocumentNode.WriteTo());
        }
    }
}
