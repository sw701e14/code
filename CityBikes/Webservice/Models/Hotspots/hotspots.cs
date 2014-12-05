using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.Hotspots
{
    public class allHotspots
    {
        public int count { get; set; }

        public List<hotspot> hotspots { get; set; }

        public allHotspots()
        {
            hotspots = new List<hotspot>();

            using (Database context = new Database())
            {
                foreach (Hotspot item in context.RunSession(session => session.GetAllHotspots()))
                {
                    count++;
                    hotspot tempHotspot = new hotspot();

                    foreach (GPSLocation gpsLoc in item.getDataPoints())
                    {
                        coordinate tempCoordinate = new coordinate();
                        tempCoordinate.latitude = gpsLoc.Latitude;
                        tempCoordinate.longtitude = gpsLoc.Longitude;
                        tempHotspot.coordinates.Add(tempCoordinate);
                    }
                    hotspots.Add(tempHotspot);
                }
            }
        }
    }
}