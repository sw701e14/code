﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.AvailableBikes
{
    public class availableBike
    {
        public long id { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }

        public availableBike() { }
    }
}