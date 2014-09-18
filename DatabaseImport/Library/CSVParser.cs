using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class CSVParser
    {
        public static GPSPoint[] GetData(string fileName, int bikeID)
        {
            string[] lines = FileHandler.LoadFile(fileName);
            GPSPoint[] data = new GPSPoint[lines.Count()-1];
            for (int i = 1; i < lines.Count(); i++)
            {
                string[] l = lines[i].Split(',');
                data[i-1] = new GPSPoint(Convert.ToDateTime(l[0]), Convert.ToDouble(l[1]), Convert.ToDouble(l[2]), Convert.ToInt32(l[5]), bikeID);
                //Console.WriteLine(Convert.ToDateTime(l[0]).ToString() + ", " + Convert.ToDouble(l[1]).ToString() + ", " + Convert.ToDouble(l[2]).ToString() + ", " + Convert.ToInt32(l[5]).ToString() + ", 1");
            }
            return data;
        }
    }
}
