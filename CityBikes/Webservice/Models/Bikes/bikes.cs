using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.AllBikes
{
    public class bikes
    {
        public int count { get; set; }
        public List<bike> bikeList { get; set; }

        public bikes()
        {
            bikeList = new List<bike>();
        }

        public class bike
        {
            public uint id { get; set; }
            public string latitude { get; set; }
            public string longtitude { get; set; }

            public string immobileSince { get; set; }

            public bike() { }
        }
    }
}