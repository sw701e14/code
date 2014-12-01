using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Hotspots
{
    public class hotspot
    {
        public uint id { get; set; }
        public int count { get; set; }

        public List<coordinate> coordinates { get; set; }

        public hotspot() 
        {
            coordinates = new List<coordinate>();
        }

        public class coordinate
        {
            public decimal latitude { get; set; }
            public decimal longtitude { get; set; }

            public coordinate() { }
        }
    }
}