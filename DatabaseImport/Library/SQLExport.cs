using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class SQLExport
    {
        private const string TABLENAME = "gpsdata";
        private const string COLUMNS = "(time,lat,long,acc,bike)";
        private static Random rnd = new Random();

        public static void Export(IEnumerable<GPSPoint> points, string filename)
        {
            using (StreamWriter output = new System.IO.StreamWriter(filename))
            {
                string insertStatement = WriteInsertStatement(points);
                output.Write(insertStatement);
            }
        }

        private static string WriteInsertStatement(IEnumerable<GPSPoint> points)
        {
            StringBuilder output = new StringBuilder();

            output.Append("INSERT INTO " + TABLENAME + "\n" + COLUMNS + "\nVALUES\n (");

            foreach (var point in points)
            {
                output.Append(point.writeGPSPoint());
            }

            output.Append(");");

            return output.ToString();
        }

        private static string writeGPSPoint(this GPSPoint point)
        {

            return string.Format("('{0}', {1}, {2}, {3}, {4})",
                            point.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
                            point.Latitude.ToString(CultureInfo.CreateSpecificCulture("en-UK")),
                            point.Longitude.ToString(CultureInfo.CreateSpecificCulture("en-UK")),
                            getAccuracy(point.Accuracy),
                            point.BikeId);


        }

        private static int getAccuracy(int? accuracy)
        {
            if (accuracy.HasValue)
                return accuracy.Value;
            else
                return rnd.Next(30);
        }
    }

}
