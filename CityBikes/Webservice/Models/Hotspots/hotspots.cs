using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Hotspots
{
    public class hotspots
    {
        public int count { get; set; }

        public List<hotspot> hotspotList { get; set; }

        public hotspots()
        {
            hotspotList = new List<hotspot>();
        }
    }
}