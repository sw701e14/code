using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Hotspot
    {
        public List<Coordinate> Coordinates { get; set; }
        public uint Id { get; set; }

        private Hotspot() { }


        public static Hotspot ConvertFromHotspot(Shared.DTO.Hotspot hotspot)
        {
            Hotspot h = new Hotspot();

            h.Id= hotspot.GetId();
            h.Coordinates = hotspot.getDataPoints().Select(x => Coordinate.ConvertFromLocation(x)).ToList();

            return h;
        }
    }
}