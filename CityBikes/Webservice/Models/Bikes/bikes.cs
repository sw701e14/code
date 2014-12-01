using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.AllBikes
{
    public class AllBikes
    {
        public int count { get; set; }
        public List<bike> bikes { get; set; }

        public AllBikes()
        {
            bikes = new List<bike>();
        }
    }
}