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
        /// <returns>A collection of <see cref="GPSPoint"/> representing the data in the file.</returns>
        public static GPSPoint[] GetData(string fileName, int bikeID)
        {
            string[] lines = File.ReadAllLines(fileName);
            GPSPoint[] data = new GPSPoint[lines.Count()-1];
            for (int i = 1; i < lines.Count(); i++)
            {
                string[] l = lines[i].Split(',');
                data[i - 1] = new GPSPoint(Convert.ToDateTime(l[0]), Convert.ToDouble(l[1], dateCulture), Convert.ToDouble(l[2], dateCulture), Convert.ToInt32(l[5]), bikeID);
            }
            return data;
        }
    }
}
