using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class CSVParser
    {
        private static CultureInfo dateCulture = CultureInfo.CreateSpecificCulture("en-UK");

        public static GPSPoint[] GetData(string fileName, int bikeID)
        {
            string[] lines = FileHandler.LoadFile(fileName);
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
