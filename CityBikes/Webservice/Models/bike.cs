using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Bike
    {
        public long Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public DateTime ImmobileSince { get; set; }

        public Bike() { }
    }
}