using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.AllBikes
{
    public class allBikes
    {
        public int count { get; set; }
        public List<bike> bikes { get; set; }

        public allBikes()
        {
            bikes = new List<bike>();
        }

        public class bike
        {
            public string href { get; set; }

            public bike() { }
        }
    }
}