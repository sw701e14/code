using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Ressources
{
    public static class ResourceAdressing
    {
        public static string GetHotspot(uint id)
        {
            return "/hotspots/" + id + "/";
        }

        public static string GetAvailableBike(uint id)
        {
            return "/availablebikes/" + id + "/";
        }
    }
}