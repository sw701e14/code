using System;
using System.Collections.Generic;
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
        public static void Export(GPSPoint[] points,string filename)
        {
            StreamWriter output = new System.IO.StreamWriter(filename);

            output.WriteLine("INSERT INTO " + TABLENAME + "\n"+COLUMNS + "\nVALUES\n");
            
            foreach (var point in points)
            {
                output.WriteLine( point.writeGPSPoint());
            }

            output.WriteLine(";");
        }

        private static string writeGPSPoint(this GPSPoint point)
        {
            return "(" + 
                point.TimeStamp + "," + 
                point.Latitude + "," + 
                point.Longitude + "," + 
                point.Accuracy == null ? "null" : point.Accuracy + "," + 
                point.BikeId + ")";
        }
    }

}
