using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Prediction
    {
        public Hotspot Hotspot { get; set; }

        public int Time { get; set; }

        public Prediction() { }
    }
}