using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.Hotspots
{
    public class AllHotspots
    {
        //public int Count { get { return Hotspots.Count; } }

        public List<Webservice.Models.Ressources.References.Hotspot> Hotspots { get; set; }
        //public List<Webservice.Models.Hotspot> Hotspots { get; set; }

        public AllHotspots()
        {
            Hotspots = Data.GetAllHotspots().Keys.Select(x=> new Webservice.Models.Ressources.References.Hotspot(x)).ToList();
        }
    }
}