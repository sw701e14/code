using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Prediction
    {
        Hotspot hotspot;
        int time;

        public Prediction(Hotspot h,int time)
        {
            this.hotspot = hotspot;
        }
    }
}