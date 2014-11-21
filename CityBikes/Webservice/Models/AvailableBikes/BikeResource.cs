using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Library;

namespace Webservice.Models.AvailableBikes
{
    public class BikeResource
    {
        public Bike bike { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }

        public BikeResource() { }
    }
}