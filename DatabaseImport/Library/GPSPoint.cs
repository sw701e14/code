using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GPSPoint
    {
        public DateTime TimeStamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int Accuracy { get; set; }
        public int BikeId { get; set; }

        public GPSPoint(DateTime timestamp, double latitude, double longitude, int accuracy, int bikeId)
        {
            this.TimeStamp = timestamp;
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Accuracy = accuracy;
            this.BikeId = bikeId;
        }
    }
}
