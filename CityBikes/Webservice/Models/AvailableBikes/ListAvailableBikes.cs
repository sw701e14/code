using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Library.GeneratedDatabaseModel;

namespace Webservice.Models.AvailableBikes
{
    public class ListAvailableBikes
    {
        public int count { get; set; }
        public List<bike> bikes { get; set; }

        public ListAvailableBikes()
        {
            bikes = new List<bike>();
        }
    }

    public class bike
    {
        public long id { get; set; }
        public string href { get; set; }

        public bike() { }
    }
}