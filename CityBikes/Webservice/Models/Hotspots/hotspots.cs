using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Hotspots
{
    public class allHotspots
    {
        public int count { get; set; }

        public List<hotspot> hotspots { get; set; }

        public allHotspots()
        {
            hotspots = new List<hotspot>();
        }
    }
}