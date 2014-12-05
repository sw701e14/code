using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class hotspot
    {
        public List<coordinate> coordinates { get; set; }

        public hotspot()
        {
            coordinates = new List<coordinate>();
        }
    }
}