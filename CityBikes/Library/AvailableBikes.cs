using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public static class AvailableBikes
    {
        public static IEnumerable<Tuple<int/*, GPSLocation*/>> GetAvailableBikes()
        {
            IEnumerable<Tuple<int, GPSLocation>> allBikesPosition; /*= FindLocationOfAllBikes();*/
            IEnumerable<Tuple<int, DateTime>> allBikesImmobile; /*= BikesImmobileFor();*/


            IEnumerable<Tuple<int, DateTime, GPSLocation>> answer = allBikesPosition.Join(allBikesImmobile, p => p.Item1, i => i.Item1, (p, i) => new {Tuple<p.Item1, p.Item2, i.Item2});



            foreach (Tuple<int/*, GPSLocation*/> bike in allBikesPosition)
            {
                if (true)
                {
                    yield return bike;
                }
            }

        }

    }
}
