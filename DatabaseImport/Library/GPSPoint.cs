using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public struct GPSPoint : IEquatable<GPSPoint>
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

        public static bool operator ==(GPSPoint p1, GPSPoint p2)
        {
            return p1.Equals(p1);
        }
        public static bool operator !=(GPSPoint p1, GPSPoint p2)
        {
            return !p1.Equals(p2);
        }

        public override int GetHashCode()
        {
            return timestamp.GetHashCode() ^ latitude.GetHashCode() ^ longitude.GetHashCode() ^ accuracy.GetHashCode() ^ bikeId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is GPSPoint)
                return Equals((GPSPoint)obj);
            else
                return false;
        }

        public bool Equals(GPSPoint other)
        {
            return this.timestamp.Equals(other.timestamp)
                && this.latitude.Equals(other.latitude)
                && this.longitude.Equals(other.longitude)
                && this.accuracy.Equals(other.accuracy)
                && this.bikeId.Equals(other.bikeId);
        }
    }
}
