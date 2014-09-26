using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.GeneratedDatabaseModel
{
    public struct GPSLocation
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
    }
}
