﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseImport;
using DeadDog.Console;
using System.IO;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!hasFiles())
                return;

            Menu<IEnumerable<GPSPoint>> menu = new Menu<IEnumerable<GPSPoint>>("Load data!");
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

        static IEnumerable<GPSPoint> loadFromFile()
        {
            Menu<IEnumerable<GPSPoint>> menu = new Menu<IEnumerable<GPSPoint>>("Select a file:");
            menu.SetCancel("Cancel");

            foreach (var f in Directory.EnumerateFiles(".", "*.txt"))
                menu.Add(Path.GetFileName(f), () => loadFromFile(f));

            return menu.Show();
        }
        static IEnumerable<GPSPoint> loadFromFile(string path)
        {
            int id = "Specify bike ID: ".GetInt32(x => x > 0);
            return CSVParser.GetData(path, id);
        }

        static IEnumerable<GPSPoint> loadFromGoogle()
        {
            string from = "From: ".GetString(x => x.Trim().Length > 0);
            string to = "To: ".GetString(x => x.Trim().Length > 0);
            DateTime start = "Start time: ".GetDateTime();
            int id = "Specify bike ID: ".GetInt32(x => x > 0);

            return GoogleDirectionsParser.GetData(from, to, start, id);
        }

        static void insertInDB(IEnumerable<IEnumerable<GPSPoint>> points)
        {
            throw new NotImplementedException();
        }
    }
}