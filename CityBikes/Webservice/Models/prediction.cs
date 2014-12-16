using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Prediction
    {
        public Hotspot Hotspot { get; private set; }

        public int Time { get; set; }

        public Prediction(Hotspot hotspot, TimeSpan time)
        {
            this.Hotspot = hotspot;
            this.Time = (int)time.TotalMinutes;
        }
    }
}