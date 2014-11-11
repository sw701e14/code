﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Library.GeneratedDatabaseModel
{
    public partial class gps_data
    {
        private static Random rnd = new Random();

        private gps_data()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="gps_data"/> class.
        /// </summary>
        /// <param name="queried">The timestamp when the measurement was recorded.</param>
        /// <param name="latitude">The latitude associated with this <see cref="gps_data"/>.</param>
        /// <param name="longitude">The longitude associated with this <see cref="gps_data"/>.</param>
        /// <param name="accuracy">The accuracy of the measurement, or <c>null</c> if no accuracy was recorded.
        /// Values with no accuracy will be stored with a random accuracy.</param>
        /// <param name="bikeId">The identifier for the bike to which this measurement belongs.</param>
        public gps_data(DateTime queried, decimal latitude, decimal longitude, int? accuracy, int bikeId)
            : this()
        {
            if (latitude > 90 || latitude < -90)
                throw new ArgumentOutOfRangeException("latitude");

            if (longitude > 180 || longitude < -180)
                throw new ArgumentOutOfRangeException("longitude");

            this.queried = queried;
            this.latitude = latitude;
            this.longitude = longitude;
            this.accuracy = getAccuracy(accuracy);
            this.bikeId = bikeId;
        }
        private static byte getAccuracy(int? accuracy)
        {
            if (!accuracy.HasValue)
                return (byte)rnd.Next(30);
            else if (accuracy.Value > 255)
                return (byte)255;
            else
                return (byte)accuracy.Value;
        }

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

        /// <summary>
        /// Moves the point the specified distance at the specified angled
        /// </summary>
        /// <param name="data">The gps_data to move</param>
        /// <param name="angle">Angle clockwise from north</param>
        /// <param name="distance">The distance in km</param>
        /// <returns></returns>
        public static gps_data Move(gps_data data, double angle, double distance)
        {
            int R = 6371; // earth radius in km
            double δ = distance / R;

            double newLatitude = Math.Asin(Math.Sin((double)data.latitude) * Math.Cos(δ) + Math.Cos(angle) * Math.Sin(δ) * Math.Cos(angle));
            double newLongitude = (double)data.longitude + Math.Atan2(Math.Sin(angle) * Math.Sin(δ) * Math.Cos((double)data.latitude), Math.Cos(δ) - Math.Sin((double)data.latitude) * Math.Sin(newLatitude));

            return new gps_data(data.queried, (decimal)newLatitude, (decimal)newLongitude, data.accuracy,(int)data.bikeId);
        }
    }
}