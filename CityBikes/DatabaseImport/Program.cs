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

            Menu<GPSData[]> menu = new Menu<GPSData[]>("Load data!");
            menu.SetCancel("Done");

            menu.Add("Load from file", loadFromFile);
            menu.Add("Load from Google Directions", loadFromGoogle);

            foreach (var set in menu.Show(true))
            {
                if (set != null)
                    using (Database database = new Database())
                        database.RunSession(session => DatabaseExport.Export(session, set));
            }
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

        static GPSData[] loadFromFile()
        {
            Menu<GPSData[]> menu = new Menu<GPSData[]>("Select a file:");
            menu.SetCancel("Cancel");

            foreach (var f in Directory.EnumerateFiles(".", "*.txt"))
                menu.Add(Path.GetFileName(f), () => loadFromFile(f));

            return menu.Show();
        }
        static GPSData[] loadFromFile(string path)
        {
            uint id = (uint)"Specify bike ID: ".GetInt32(x => x >= 0);
            return CSVParser.GetData(path, new Bike(id));
        }

        static GPSData[] loadFromGoogle()
        {
            string from = "From: ".GetString(x => x.Trim().Length > 0);
            string to = "To: ".GetString(x => x.Trim().Length > 0);
            DateTime start = "Start time: ".GetDateTime();
            uint id = (uint)"Specify bike ID: ".GetInt32(x => x >= 0);

            return GoogleDirectionsParser.GetData(from, to, start, new Bike(id)).ToArray();
        }

        static void insertInDB(IEnumerable<GPSData> points)
        {
            using (var database = new Database())
                database.RunSession(session => DatabaseExport.Export(session, points));
        }
    }
}
