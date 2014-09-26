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
        public static IEnumerable<Tuple<int, GPSLocation>> GetAvailableBikes()
        {
            IEnumerable<Tuple<int, GPSLocation>> allBikesPosition = FindLocationOfAllBikes();
            IEnumerable<Tuple<int, DateTime>> allBikesImmobile = AllBikesImmobileFor();
            IEnumerable<Tuple<int, GPSLocation>> allStations = FindAllSations();

            var allBikes = allBikesPosition.Join(allBikesImmobile, p => p.Item1, i => i.Item1, (p, i) => new {tuple = Tuple.Create(p.Item1, p.Item2, i.Item2)});

            foreach (var bike in allBikes)
            {
                if (AtAStation(bike.tuple.Item2))
                {
                    if (bike.tuple.Item3 > AVAILABLE_AFTER_AT_STATION)
                        yield return Tuple.Create(bike.tuple.Item1, bike.tuple.Item2);
                }
                else
                {
                    if (bike.tuple.Item3 > AVAILABLE_AFTER_NOT_AT_STATION)
                        yield return Tuple.Create(bike.tuple.Item1, bike.tuple.Item2);
                }
            }

        }

    }
}
