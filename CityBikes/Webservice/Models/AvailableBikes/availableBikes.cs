using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.AvailableBikes
{
    public class availableBikes
    {
        public int count { get; set; }
        public List<availableBike> bikes { get; set; }

        public availableBikes()
        {
            bikes = new List<availableBike>();
        }

        public class availableBike
        {
            public long id { get; set; }
            public string href { get; set; }

            public availableBike() { }
        }
    }
}