using Library.GeneratedDatabaseModel;
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
        public static gps_data[] GetData(string fileName, int bikeID)
        {
            string[] lines = File.ReadAllLines(fileName);
            gps_data[] data = new gps_data[lines.Count() - 1];
            for (int i = 1; i < lines.Count(); i++)
            {
                string[] l = lines[i].Split(',');

                DateTime timestamp = DateTime.Parse(l[0]);
                double latitude = Convert.ToDouble(l[1].Replace('.', ','));
                double longitude = Convert.ToDouble(l[2].Replace('.', ','));
                int? accuracy = parseInt(l[7], null);

                data[i - 1] = new gps_data(timestamp, (decimal)latitude, (decimal)longitude, accuracy, bikeID);
            }
            return data;
        }

        private static int? parseInt(string text, int? no_val)
        {
            int temp;
            return int.TryParse(text, out temp) ? temp : no_val;
        }
    }
}
