using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [Serializable]
    public struct GPSLocation : IEquatable<GPSLocation>
    {
        private const double RADIUS_OF_EARTH_IN_KM = 6371.0;

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

        public override string ToString()
        {
            return string.Format("(Lat: {0}, Long: {1})", latitude, longitude);
        }

        public override int GetHashCode()
        {
            return latitude.GetHashCode() ^ longitude.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GPSLocation))
                return false;
            else
                return Equals((GPSLocation)obj);
        }
        public bool Equals(GPSLocation other)
        {
            return latitude == other.Latitude && longitude == other.Longitude;
        }

        public static bool operator ==(GPSLocation g1, GPSLocation g2)
        {
            return g1.Equals(g2);
        }
        public static bool operator !=(GPSLocation g1, GPSLocation g2)
        {
            return !g1.Equals(g2);
        }

        public static GPSLocation operator +(GPSLocation g1, GPSLocation g2)
        {
            return new GPSLocation(g1.latitude + g2.latitude, g1.longitude + g2.longitude);
        }
        public static GPSLocation operator -(GPSLocation g1, GPSLocation g2)
        {
            return new GPSLocation(g1.latitude - g2.latitude, g1.longitude - g2.longitude);
        }

        /// <summary>
        /// Calculates the distance (in meters) between this <see cref="GPSLocation"/> and another.
        /// See <see cref="GetDistance"/> for more.
        /// </summary>
        /// <param name="location">The location to include in the calculation.</param>
        /// <returns>The distance (in meters) between the two locations.</returns>
        public double DistanceTo(GPSLocation location)
        {
            return GetDistance(this, location);
        }
        /// <summary>
        /// Calculates the distance (in meters) between two <see cref="GPSLocation"/>s.
        /// </summary>
        /// <param name="gps1">The first location to use in the calculation.</param>
        /// <param name="gps2">The second location to use in the calculation.</param>
        /// <returns>The distance (in meters) between the two locations.</returns>
        /// <remarks>Implemented using the description at http://www.movable-type.co.uk/scripts/latlong.html</remarks>
        public static double GetDistance(GPSLocation gps1, GPSLocation gps2)
        {
            var φ1 = degreeToRadians(gps1.latitude);
            var φ2 = degreeToRadians(gps2.latitude);
            var Δφ = degreeToRadians(gps2.latitude - gps1.latitude);
            var Δλ = degreeToRadians(gps2.longitude - gps1.longitude);

            var a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                    Math.Cos(φ1) * Math.Cos(φ2) *
                    Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return RADIUS_OF_EARTH_IN_KM * c * 1000; //Converting to meters by multiplying with 1000
        }

        private static double degreeToRadians(decimal value)
        {
            return (Math.PI * (double)value) / 180.0;
        }
    }
}
