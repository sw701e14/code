using Library;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseImport
{
    /// <summary>
    /// Exposes methods for parsing a csv file with gps info.
    /// </summary>
    public static class CSVParser
    {
        private static CultureInfo dateCulture = CultureInfo.CreateSpecificCulture("en-UK");

        /// <summary>
        /// Gets GPS data from a CSV file and uses a predefined id for the bike.
        /// </summary>
        /// <param name="fileName">The name of the file from which data should be loaded.</param>
        /// <param name="bikeID">The bike identifier.</param>
        /// <returns>A collection of <see cref="gps_data"/> representing the data in the file.</returns>
        public static GPSData[] GetData(string fileName, Bike bike)
        {
            string[] lines = File.ReadAllLines(fileName);
            GPSData[] data = new GPSData[lines.Count() - 1];
            for (int i = 1; i < lines.Count(); i++)
            {
                string[] l = lines[i].Split(',');

                DateTime timestamp = DateTime.Parse(l[0]);
                double latitude = Convert.ToDouble(l[1].Replace('.', ','));
                double longitude = Convert.ToDouble(l[2].Replace('.', ','));
                byte? accuracy = parseByte(l[7], null);

                var loc = new GPSLocation((decimal)latitude, (decimal)longitude);
                data[i - 1] = new GPSData(bike, loc, accuracy, timestamp, false);
            }
            return data;
        }

        private static byte? parseByte(string text, byte? no_val)
        {
            byte temp;
            return byte.TryParse(text, out temp) ? temp : no_val;
        }
    }
}
