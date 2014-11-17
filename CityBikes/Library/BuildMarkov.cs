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
                            var v = hotspots.Where(x => IsInConvexHull(x, gps_data));
                            if (v.Count() > 1)
                                throw new InvalidOperationException("a point should not be able to be in more than one hotspot at a time");
                            GPSLocation[] h = v.First();
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
                            var v = hotspots.Where(x => IsInConvexHull(x, gps_data));
                            if (v.Count() > 1)
                                throw new InvalidOperationException("a point should not be able to be in more than one hotspot at a time");
                            GPSLocation[] h = v.First();
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

        /// <summary>
        /// Determines whether testpoint is in the hotspot defined by the specified polygon.
        /// </summary>
        /// <param name="polygon">The polygon.</param>
        /// <param name="testPoint">The test point.</param>
        /// <returns>true if testPoint is in the hotspot</returns>
        private bool IsInConvexHull(GPSLocation[] polygon, gps_data testPoint)
        {
            //inspired by http://stackoverflow.com/a/14998816
            bool result = false;
            int j = polygon.Count() - 1;
            for (int i = 0; i < polygon.Count(); i++)
            {
                if (polygon[i].Latitude < testPoint.latitude && polygon[j].Latitude >= testPoint.latitude || polygon[j].Latitude < testPoint.latitude && polygon[i].Latitude >= testPoint.latitude)
                {
                    if (polygon[i].Longitude + (testPoint.latitude - polygon[i].Latitude) / (polygon[j].Latitude - polygon[i].Latitude) * (polygon[j].Longitude - polygon[i].Longitude) < testPoint.longitude)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
        }
    }
}
