using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Hotspot
    {
        public List<Coordinate> Coordinates { get; set; }

        public Hotspot()
        {
            Coordinates = new List<Coordinate>();
        }
    }
}