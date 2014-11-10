using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public class BuildMarkov
    {
        private List<GPSLocation[]> hotspots;

        public List<GPSLocation[]> Hotspots
        {
            get { return hotspots; }
            set { hotspots = value; }
        }

        public MarkovChain Build(Database context)
        {
            hotspots = context.LoadHotSpotsFromDatabase();

            var routes = from gps_data in context.gps_data
                         group gps_data by gps_data.bikeId into bike
                         select bike;


            MarkovChain mc = new MarkovChain(hotspots.Count() * 2, null);

            int oldIndex;

            foreach (var bike in routes)
            {
                oldIndex = -1;
                foreach (var gps_data in bike)
                {
                    if (oldIndex == -1)
                    {
                        if (hotspots.Any(x => IsInConvexHull(x, gps_data))) // destination is hotspot
                        {
                            GPSLocation[] h = hotspots.First(x => IsInConvexHull(x, gps_data));
                            oldIndex = getHotspotIndex(h, hotspots);
                        }
                        else //default initial??
                        {
                            oldIndex = 0; // find closest cluster
                        }
                    }
                    else
                    {

                        if (hotspots.Any(x => IsInConvexHull(x, gps_data))) // destination is hotspot
                        {
                            GPSLocation[] h = hotspots.First(x => IsInConvexHull(x, gps_data));
                            int destinationIndex = getHotspotIndex(h, hotspots);
                            mc[oldIndex, destinationIndex] += 1;
                            oldIndex = destinationIndex;
                        }
                        else // destination is not hotspot
                        {

                            if (oldIndex % 2 == 0) // oldindex was hotspot
                            {
                                int destinationIndex = oldIndex + 1;
                                mc[oldIndex, oldIndex + 1] += 1;
                                oldIndex = destinationIndex;
                            }
                            else // oldindex was not hotspot
                            {
                                mc[oldIndex, oldIndex] += 1;

                            }

                        }
                    }

                }
            }

            return mc;
        }

        private int getHotspotIndex(GPSLocation[] hotspot, List<GPSLocation[]> hotspots)
        {
            return hotspots.IndexOf(hotspot) * 2;
        }

        private int getHotSpotLeftIndex(GPSLocation[] hotspot, List<GPSLocation[]> hotspots)
        {
            return hotspots.IndexOf(hotspot) * 2 + 1;
        }

        private bool IsInConvexHull(GPSLocation[] poly, gps_data point)
        {
            // Inspired by http://stackoverflow.com/a/4243079
            var coef = poly.Skip(1).Select((p, i) =>
                                            (point.longitude - poly[i].Latitude) * (p.Longitude - poly[i].Longitude)
                                          - (point.longitude - poly[i].Longitude) * (p.Latitude - poly[i].Latitude))
                                    .ToList();

            if (coef.Any(p => p == 0))
                return true;

            for (int i = 1; i < coef.Count(); i++)
            {
                if (coef[i] * coef[i - 1] < 0)
                    return false;
            }
            return true;
        }   
    }
}
