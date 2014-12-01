using Library;
using System;
using System.Collections.Generic;

namespace DataLoading.Common
{
   public static class GPSInputExtension
   {
       public static IEnumerable<GPSData> ConvertToInterval(this IEnumerable<GPSData> route, TimeSpan interval)
       {
           if (route == null)
               throw new ArgumentNullException("route");

           if (interval.TotalSeconds <= 0.0)
               throw new ArgumentOutOfRangeException("interval", "The interval used for conversion must be strictly positive.");

           var e = route.GetEnumerator();

           if (!e.MoveNext())
           {
               e.Dispose();
               yield break;
           }

           Bike bike = e.Current.Bike;

           var lastPoint = e.Current;
           yield return lastPoint;
           DateTime nextTime = lastPoint.QueryTime.Add(interval).addRandomSeconds(60);

           while (e.MoveNext())
           {
               var nextPoint = e.Current;

               while (nextPoint.QueryTime >= nextTime)
               {
                   var point = generateBetweenPoint(lastPoint, nextPoint, nextTime);
                   yield return new GPSData(bike, point, null, nextTime);
                   nextTime = nextTime.Add(interval).addRandomSeconds(60);
               }

               lastPoint = nextPoint;
           }

           e.Dispose();
       }
       private static DateTime addRandomSeconds(this DateTime dt, double secondsRange)
       {
           return dt.AddSeconds((r.NextDouble() - 0.5) * secondsRange);
       }
       private static GPSLocation generateBetweenPoint(GPSData lp, GPSData np, DateTime time)
       {
           var diff = (np.QueryTime - lp.QueryTime);

           double triptime = diff.TotalSeconds;
           double pointtime = (time - lp.QueryTime).TotalSeconds;

           if (pointtime != 0)
               return lp.Location + (np.Location - lp.Location) * (pointtime / triptime);
           else
               return lp.Location;
       }

       
       private static Random r = new Random();
       public static GPSData Randomize(this GPSData point)
       {
           double angle = r.NextDouble() * 2 * Math.PI;
           double distance = r.NextDouble() * (double)point.Accuracy;

           distance /= 1000.0; // Conversion to kilometers

           return new GPSData(point.Bike, GPSLocation.Move(point.Location, angle, distance), point.Accuracy, point.QueryTime, point.HasNotMoved);
       }
       public static IEnumerable<GPSData> Randomize(this IEnumerable<GPSData> points)
       {
           foreach (var p in points)
               yield return Randomize(p);
       }
    }
}
