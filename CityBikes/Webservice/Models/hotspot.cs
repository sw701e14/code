using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Hotspot
    {
        public Coordinate[] Coordinates { get; private set; }

        public Hotspot(IEnumerable<Coordinate> coordinates)
        {
            this.Coordinates = coordinates.ToArray();
        }

        public static Hotspot ConvertFromHotspot(Shared.DTO.Hotspot hotspot)
        {
            return new Hotspot(hotspot.getDataPoints().Select(x => Coordinate.ConvertFromLocation(x));
        }
    }
}