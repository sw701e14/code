using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GPSPoint
    {
        public DateTime TimeStamp { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public int? Accuracy { get; private set; }
        public int BikeId { get; private set; }

        public GPSPoint(DateTime timestamp, double latitude, double longitude, int? accuracy, int bikeId)
        {
            this.TimeStamp = timestamp;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Accuracy = accuracy;
            this.BikeId = bikeId;
        }
    }
}
