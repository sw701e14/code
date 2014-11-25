using Library;
using System;
using System.Collections.Generic;

namespace DataCollector
{
   public static class GPSDataExtension
   {
       public static IEnumerable<GPSData> ConvertToInterval(this IEnumerable<GPSData> route, TimeSpan interval)
       {
           var e = route.GetEnumerator();

           if (!e.MoveNext())
           {
               e.Dispose();
               yield break;
           }

           Bike bike = e.Current.Bike;

           var lastPoint = e.Current;
           yield return lastPoint;
           DateTime nextTime = lastPoint.QueryTime.Add(interval);

           while (e.MoveNext())
           {
               var nextPoint = e.Current;

               while (nextPoint.QueryTime >= nextTime)
               {
                   var point = generateBetweenPoint(lastPoint, nextPoint, nextTime);
                   yield return new GPSData(bike, point, null, nextTime);
                   nextTime = nextTime.Add(interval);
               }

               lastPoint = nextPoint;
           }

           e.Dispose();
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
    }
}
