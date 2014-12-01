using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Hotspots
{
    public class AllHotspots
    {
        public int count { get; set; }

        public List<hotspot> hotspots { get; set; }

        public AllHotspots()
        {
            hotspots = new List<hotspot>();
        }
    }
}