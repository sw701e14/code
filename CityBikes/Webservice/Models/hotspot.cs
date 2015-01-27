using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Hotspot
    {
        public List<Coordinate> Coordinates { get; private set; }
        public uint Id { get; set; }

        private Hotspot() { }

        public Hotspot(uint id, IEnumerable<Coordinate> coordinates)
        {
            Id = id;
            this.Coordinates = coordinates.ToList();
        }

        public static Hotspot ConvertFromHotspot(Shared.DTO.Hotspot hotspot)
        {
            return new Hotspot(hotspot.GetId(hotspot), hotspot.getDataPoints().Select(x => Coordinate.ConvertFromLocation(x)));
        }
    }
}