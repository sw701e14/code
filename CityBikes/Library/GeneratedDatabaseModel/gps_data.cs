﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.GeneratedDatabaseModel
{
    public partial class gps_data
    {
        public GPSLocation Location
        {
            get { return new GPSLocation(this.latitude, this.longitude); }
            set { this.latitude = value.Latitude; this.longitude = value.Longitude; }
        }

        public static bool inVicinity(gps_data d1, gps_data d2)
        {
            double dist = GPSLocation.Distance(d1.Location, d2.Location) * 1000;
            return d1.accuracy + d2.accuracy >= dist;
        }
    }
}