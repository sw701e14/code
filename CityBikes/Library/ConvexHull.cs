using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    /// The algorithm in this class is implemented as described in the book
    /// Introduction to algorithms
    /// by Cormen, Thomas H and Leiserson, Charles E and Rivest, Ronald L and Stein, Clifford and others
    /// published by MIT press Cambridge
    public static class ConvexHull
    {
        



        /// <summary>
        /// Performs a grahamscan on the specified array of gpslocations
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The set of the points that make up the convex hull</returns>
        public static GPSLocation[] GrahamScan(IEnumerable<GPSLocation> data)
        {
            GPSLocation p0 = data.Aggregate((minItem, nextItem) => minItem.Longitude < nextItem.Longitude ? minItem : nextItem);
            GPSLocation[] remaining = data.Where(x => !x.Equals(p0)).OrderBy(x => computePolarAngle(p0, x)).ToArray();

            Stack<GPSLocation> stack = new Stack<GPSLocation>();
            stack.Push(p0);
            stack.Push(remaining[0]);
            stack.Push(remaining[1]);

            for (int i = 2; i < remaining.Count(); i++)
            {
                while (!isLeftTurn(stack.ElementAt(1), stack.ElementAt(0), remaining[i]))
                {
                    stack.Pop();
                }
                stack.Push(remaining[i]);
            }

            return stack.ToArray();
        }

        /// <summary>
        /// Computes the polar angle.
        /// </summary>
        /// <param name="origin">The origin.</param>
        /// <param name="point">The point.</param>
        /// <returns></returns>
        private static double computePolarAngle(GPSLocation origin, GPSLocation point)
        {
            decimal longitude = point.Longitude - origin.Longitude;
            decimal latitude = point.Latitude - origin.Latitude;
            return Math.Atan2((double)latitude, (double)longitude);

        }

        /// <summary>
        /// Computes the crossproduct of the two vectors p1-origin and p2-origin
        /// </summary>
        /// <param name="p1">The p1 point</param>
        /// <param name="p2">The p2 point</param>
        /// <param name="origin">The origin point to compute the vector from</param>
        /// <returns>The crossproduct of the two vectors </returns>
        private static decimal computeCrossProduct(GPSLocation p1, GPSLocation p2, GPSLocation origin)
        {
            GPSLocation g = p1 - origin;
            GPSLocation g2 = p2 - origin;

            return g.Latitude * g2.Longitude - g2.Latitude * g.Longitude;
        }

        /// <summary>
        /// Determines whether the turn from the origin through p1 to p2 is a left turn 
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        private static bool isLeftTurn(GPSLocation p1, GPSLocation p2, GPSLocation origin)
        {

            return 0 > computeCrossProduct(p1, p2, origin);
        }
    }
}