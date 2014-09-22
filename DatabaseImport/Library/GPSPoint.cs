using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class GPSPoint
    {
        private readonly DateTime timestamp;
        private readonly double latitude, longitude;
        private readonly int? accuracy;
        private readonly int bikeId;

        public DateTime TimeStamp
        {
            get { return timestamp; }
        }
        public double Latitude
        {
            get { return latitude; }
        }
        public double Longitude
        {
            get { return longitude; }
        }
        public int? Accuracy
        {
            get { return accuracy; }
        }
        public int BikeId
        {
            get { return bikeId; }
        }

        public GPSPoint(DateTime timestamp, double latitude, double longitude, int? accuracy, int bikeId)
        {
            this.timestamp = timestamp;
            this.latitude = latitude;
            this.longitude = longitude;
            this.accuracy = accuracy;
            this.bikeId = bikeId;
        }
    }
}
