using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class prediction
    {
        hotspot hotspot;
        int time;

        public prediction(hotspot h,int time)
        {
            this.hotspot = hotspot;
        }
    }
}