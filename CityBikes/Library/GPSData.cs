﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public struct GPSData
    {
        private static Random rnd = new Random();

        private Bike bike;
        private GPSLocation location;
        private byte accuracy;
        private DateTime queryTime;
        private bool hasNotMoved;

        public GPSData(Bike bike, GPSLocation location, byte? accuracy, DateTime queryTime, bool hasNotMoved)
        {
            this.bike = bike;
            this.location = location;
            this.accuracy = getAccuracy(accuracy);
            this.queryTime = queryTime;
            this.hasNotMoved = hasNotMoved;
        }

        private static byte getAccuracy(byte? accuracy)
        {
            if (!accuracy.HasValue)
                return (byte)rnd.Next(30);
            else
                return accuracy.Value;
        }

        /// <summary>
        /// Determines if two <see cref="GPSData"/> points are equal, with respect to each of their accuracies.
        /// </summary>
        /// <param name="d1">The first data-point.</param>
        /// <param name="d2">The second data-point.</param>
        /// <returns><c>true</c> if the two points are within range of their respective accuracies; otherwise <c>false</c>.</returns>
        public static bool WithinAccuracy(GPSData d1, GPSData d2)
        {
            double dist = GPSLocation.GetDistance(d1.Location, d2.Location) * 1000;
            return d1.accuracy + d2.accuracy >= dist;
        }

        public Bike Bike
        {
            get { return bike; }
        }
        public GPSLocation Location
        {
            get { return location; }
        }
        public byte Accuracy
        {
            get { return accuracy; }
        }
        public DateTime QueryTime
        {
            get { return queryTime; }
        }
        public bool HasNotMoved
        {
            get { return hasNotMoved; }
        }
    }
}
