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
        public int count { get { return hotspots.Count; } }

        public List<hotspot> hotspots { get; set; }

        public allHotspots()
        {
            hotspots = new List<hotspot>();

            hotspots.AddRange(Data.GetAllHotspots());
            
        }
    }
}