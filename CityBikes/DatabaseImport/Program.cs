using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseImport;
using DeadDog.Console;
using System.IO;
using Library;

namespace Run
{
    static class Program
    {
        static void Main(string[] args)
        {
            generateGPSDataTest();
            Menu menu = new Menu("Select an option");

            menu.Add("Load data", loadData);
            menu.Add("Clear data", clearData);

            menu.SetCancel("Exit");

            menu.Show(true);
        }

        static void clearData()
        {
            using (Database database = new Database())
                database.RunSession(session => session.Execute("TRUNCATE TABLE gps_data"));
        }

        static void loadData()
        {
            if (!hasFiles())
                return;

            Menu<IEnumerable<GPSData>> menu = new Menu<IEnumerable<GPSData>>("Load data!");
            menu.SetCancel("Done");

            menu.Add("Load from file", loadFromFile);
            menu.Add("Load from Google Directions", loadFromGoogle);

            var points = menu.Show(true).ToArray();

            if (points == null || points.Count() == 0)
                return;

            if (points.All(x => x == null))
                return;

            Menu insertIn = new Menu("Insert into");
            insertIn.SetCancel("Do nothing!");

            insertIn.Add("Insert into database", () => insertInDB(points));

            insertIn.Show();
        }

        static bool hasFiles()
        {
            if (!Directory.EnumerateFiles(".", "*.txt").Any())
            {
                Console.WriteLine("Unable to find any .txt files in working directory.");
                Console.WriteLine("Make sure that your projects working directory,");
                Console.WriteLine("is set to the directory with the test-files.");
                Console.WriteLine();
                Console.WriteLine("The current working dir is:");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("    {0}\\", Path.GetFullPath("."));
                Console.ForegroundColor = ConsoleColor.Gray;

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("Press any key to exit application.");
                Console.ReadKey(true);
                return false;
            }
            return true;
        }

        static IEnumerable<GPSData> loadFromFile()
        {
            Menu<IEnumerable<gps_data>> menu = new Menu<IEnumerable<gps_data>>("Select a file:");
            menu.SetCancel("Cancel");

            foreach (var f in Directory.EnumerateFiles(".", "*.txt"))
                menu.Add(Path.GetFileName(f), () => loadFromFile(f));

            return menu.Show();
        }
        static IEnumerable<GPSData> loadFromFile(string path)
        {
            int id = "Specify bike ID: ".GetInt32(x => x > 0);
            return CSVParser.GetData(path, id);
        }

        static IEnumerable<GPSData> loadFromGoogle()
        {
            string from = "From: ".GetString(x => x.Trim().Length > 0);
            string to = "To: ".GetString(x => x.Trim().Length > 0);
            DateTime start = "Start time: ".GetDateTime();
            int id = "Specify bike ID: ".GetInt32(x => x > 0);

            return GoogleDirectionsParser.GetData(from, to, start, id);
        }

        static void insertInDB(IEnumerable<IEnumerable<GPSData>> points)
        {
            using (var database = new Database())
                DatabaseExport.Export(database, points.Concatenate());
        }

        static IEnumerable<T> Concatenate<T>(this IEnumerable<IEnumerable<T>> collection)
        {
            foreach (var c in collection)
                if (c != null)
                    foreach (var t in c)
                        yield return t;
        }

        static void generateGPSDataTest()
        {
            string[] bycykelstations = { "karolinelund 9000 aalborg", "strandvejen 9000 aalborg", "havnefronten 9000 aalborg", "vestbyens station 9000 aalborg", 
                                           "Utzon Center Slotspladsen 4 9000 Aalborg", "nytorv 9000 aalborg", "algade 9000 aalborg", "gammeltorv 9000 aalborg", 
                                           "aalborg zoo 9000 aalborg", "fibigerstræde 9000 aalborg", "kjellerups torv 9000 aalborg", "friis 9000 aalborg", 
                                           "aalborg hallen 9000 aalborg", "aalborg banegård 9000 aalborg", "kunsten 9000 aalborg", "haraldslund 9000 aalborg", 
                                           "nørresundby torv 9400 nørresundby", "vestergade 9400 nørresundby" };

            string[] addresses = { "Borgmester Jørgensensvej 5 9000 aalborg", "Selma Lagerlöfsvej 300 9220 aalborg", "sønderbro 25 9000 aalborg", 
                                     "langesgade 3 9000 aalborg", "kayerødsgade 10 9000 aalborg", "toldstrupsgade 14 9000 aalborg", "danmarksgade 30 9000 aalborg",
                                     "christiansgade 44 9000 aalborg", "sankelmarksgade 33 9000 aalborg", "vesterbro 30 9000 aalborg", "prinsensgade 4 9000 aalborg" };

            string[] destinations = bycykelstations.Concat(addresses).ToArray();

            List<gps_data> points = GenerateGPSData.GenerateBikeRoutes(100, destinations, new DateTime(2014, 1, 1, 8, 0, 0), 20).ToList();

            SQLExport.Export(points, "testdata2", false);

            GPSPointMapPlotter.SaveMapAsHtml(new Database(), points.AsQueryable());
            Console.ReadKey();
        }
    }
}
