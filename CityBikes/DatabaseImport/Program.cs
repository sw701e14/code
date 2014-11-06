using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseImport;
using DeadDog.Console;
using System.IO;
using Library.GeneratedDatabaseModel;

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
                database.ExecuteStoreCommand("TRUNCATE TABLE gps_data");
        }

        static void loadData()
        {
            if (!hasFiles())
                return;

            Menu<IEnumerable<gps_data>> menu = new Menu<IEnumerable<gps_data>>("Load data!");
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
            insertIn.Add("Write to file", () =>
            {
                string filename = "Filename (.sql is appended automatically): ".GetString() + ".sql";

                if (File.Exists(filename))
                    File.Delete(filename);

                foreach (var p in points)
                    if (p != null)
                        SQLExport.Export(p, filename, true);
            });

            insertIn.Show();*/
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

        static IEnumerable<gps_data> loadFromFile()
        {
            Menu<IEnumerable<gps_data>> menu = new Menu<IEnumerable<gps_data>>("Select a file:");
            menu.SetCancel("Cancel");

            foreach (var f in Directory.EnumerateFiles(".", "*.txt"))
                menu.Add(Path.GetFileName(f), () => loadFromFile(f));

            return menu.Show();
        }
        static IEnumerable<gps_data> loadFromFile(string path)
        {
            int id = "Specify bike ID: ".GetInt32(x => x > 0);
            return CSVParser.GetData(path, id);
        }

        static IEnumerable<gps_data> loadFromGoogle()
        {
            string from = "From: ".GetString(x => x.Trim().Length > 0);
            string to = "To: ".GetString(x => x.Trim().Length > 0);
            DateTime start = "Start time: ".GetDateTime();
            int id = "Specify bike ID: ".GetInt32(x => x > 0);

            return GoogleDirectionsParser.GetData(from, to, start, id);
        }

        static void insertInDB(IEnumerable<IEnumerable<gps_data>> points)
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
    }
}
