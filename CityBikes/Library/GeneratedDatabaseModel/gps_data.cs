﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.GeneratedDatabaseModel
{
    public partial class gps_data
    {
        /// <summary>
        /// Gets or sets the location this <see cref="gps_data"/> element.
        /// Setting this property will set both latitude and longitude of the element.
        /// </summary>
        public GPSLocation Location
        {
            get { return new GPSLocation(this.latitude, this.longitude); }
            set { this.latitude = value.Latitude; this.longitude = value.Longitude; }
        }

        /// <summary>
        /// Determines if two <see cref="gps_data"/> points are equal, with respect to each of their accuracies.
        /// </summary>
        /// <param name="d1">The first data-point.</param>
        /// <param name="d2">The second data-point.</param>
        /// <returns><c>true</c> if the two points are within range of their respective accuracies; otherwise <c>false</c>.</returns>
        public static bool WithinAccuracy(gps_data d1, gps_data d2)
        {
            double dist = GPSLocation.Distance(d1.Location, d2.Location) * 1000;
            return d1.accuracy + d2.accuracy >= dist;
        }
    }
}