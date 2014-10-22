using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class ConvexHull
    {
        public static GPSLocation[] GrahamScan(IEnumerable<GPSLocation> data)
        {
            GPSLocation p0 = data.Aggregate((minItem, nextItem) => minItem.Longitude < nextItem.Longitude ? minItem : nextItem);
            GPSLocation[] remaining = data.Where(x => !x.Equals(p0)).OrderBy(x => polarAngle(p0, x)).ToArray();


            Stack<GPSLocation> stack = new Stack<GPSLocation>();
            stack.Push(p0);
            stack.Push(remaining[0]);
            stack.Push(remaining[1]);

            for (int i = 2; i < data.Count(); i++)
            {
                while (!isLeftTurn(stack.ElementAt(1), stack.ElementAt(0), remaining[i]))
                {
                    stack.Pop();
                }
                stack.Push(remaining[i]);
            }

            return stack.ToArray();
        }

        private static double polarAngle(GPSLocation origin, GPSLocation point)
        {

            decimal longitude = point.Longitude - origin.Longitude;
            decimal latitude = point.Latitude - point.Latitude;
            return Math.Atan2((double)longitude, (double)latitude);


        }

        private static decimal crossProduct(GPSLocation p1, GPSLocation p2, GPSLocation origin)
        {
            GPSLocation g = p1 - origin;
            GPSLocation g2 = p2 - origin;

            return g.Latitude * g2.Longitude - g2.Latitude * g.Longitude;
        }

        private static bool isLeftTurn(GPSLocation p1, GPSLocation p2, GPSLocation origin)
        {

            return 0 > crossProduct(p1, p2, origin);
        }
    }
}