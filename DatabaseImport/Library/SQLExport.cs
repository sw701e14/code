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
        public const string TABLENAME = "gpsdata";
        public const string COLUMNS = "(time,lat,long,acc,bike)";
        public static void Export(IEnumerable<GPSPoint> points, string filename)
        {
            using (StreamWriter output = new System.IO.StreamWriter(filename))
            {

                string insertStatement = WriteInsertStatement(points);
                output.Write(insertStatement);
            }
        }

        public static string  WriteInsertStatement(IEnumerable<GPSPoint> points)
        {
            StringBuilder output = new StringBuilder();
            
            output.Append( "INSERT INTO " + TABLENAME + "\n" + COLUMNS + "\nVALUES\n (");

            foreach (var point in points)
            {
                output.Append(point.writeGPSPoint());
            }

            output.Append(");");

            return output.ToString();
        }

        public static string writeGPSPoint(this GPSPoint point)     
        {

            return string.Format("('{0}', {1}, {2}, {3}, {4})",
                            point.TimeStamp.ToString("yyyy-MM-dd HH:mm:ss"),
                            point.Latitude.ToString(CultureInfo.CreateSpecificCulture("da-DK")),
                            point.Longitude.ToString(CultureInfo.CreateSpecificCulture("da-DK")),
                            point.Accuracy.HasValue ? point.Accuracy.Value.ToString() : "null",
                            point.BikeId);


        }
    }

}
