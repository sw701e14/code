using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Ressources.References
{
    public class Hotspot : Webservice.Models.Root.RootResource.Resource
    {
        public uint id { get; set; }

        public Hotspot() : base() { }

        public Hotspot(uint id) : this() { this.id = id; this.href = ResourceAdressing.GetHotspot(id); }
    }
}