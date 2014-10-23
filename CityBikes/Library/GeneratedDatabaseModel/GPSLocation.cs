using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.GeneratedDatabaseModel
{
    public struct GPSLocation : IEquatable<GPSLocation>
    {
        private decimal latitude;
        private decimal longitude;

        public GPSLocation(decimal latitude, decimal longitude)
        {
            if (latitude < -90 || longitude > 90)
                throw new ArgumentOutOfRangeException("latitude");

            if (longitude < -180 || longitude > 180)
                throw new ArgumentOutOfRangeException("longitude");

            this.latitude = latitude;
            this.longitude = longitude;
        }

        public decimal Latitude
        {
            get { return latitude; }
            set
            {
                if (value < -90 || value > 90)
                    throw new ArgumentOutOfRangeException("latitude");

                this.latitude = value;
            }
        }
        public decimal Longitude
        {
            get { return longitude; }
            set
            {
                if (value < -180 || value > 180)
                    throw new ArgumentOutOfRangeException("longitude");

                this.longitude = value;
            }
        }

		public bool Equals(GPSLocation other)
        {
            return latitude == other.Latitude && longitude == other.Longitude;
        }

        public static GPSLocation operator -(GPSLocation g1, GPSLocation g2)
        {
            return new GPSLocation(g1.latitude - g2.latitude, g1.longitude - g2.longitude);
        }

        public override string ToString()
        {
            return string.Format("(Lat: {0}, Long: {1})", latitude, longitude);
        }
    }
}
