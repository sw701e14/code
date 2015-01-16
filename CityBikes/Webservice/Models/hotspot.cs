using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Hotspot
    {
        public List<Coordinate> Coordinates { get; private set; }

        private Hotspot() { }

        public Hotspot(IEnumerable<Coordinate> coordinates)
        {
            this.Coordinates = coordinates.ToList();
        }

        public static Hotspot ConvertFromHotspot(Shared.DTO.Hotspot hotspot)
        {
            return new Hotspot(hotspot.getDataPoints().Select(x => Coordinate.ConvertFromLocation(x)));
        }
    }
}