using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
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

        public static GPSLocation operator *(GPSLocation location, double scalar)
        {
            return location * (decimal)scalar;
        }
        public static GPSLocation operator /(GPSLocation location, double scalar)
        {
            return location / (decimal)scalar;
        }

        public static GPSLocation operator *(GPSLocation location, decimal scalar)
        {
            return new GPSLocation(location.latitude * scalar, location.longitude * scalar);
        }
        public static GPSLocation operator /(GPSLocation location, decimal scalar)
        {
            return new GPSLocation(location.latitude / scalar, location.longitude / scalar);
        }

        /// <summary>
        /// Moves the point the specified distance at the specified angled
        /// </summary>
        /// <param name="point">The gps_data to move</param>
        /// <param name="angle">Angle clockwise from north</param>
        /// <param name="distance">The distance in km</param>
        /// <returns></returns>
        public static GPSLocation Move(GPSLocation point, double angle, double distance)
        {
            int R = 6371; // earth radius in km
            double δ = distance / R;

            double oldLatitide = (Math.PI * (double)point.Latitude) / 180;
            double oldLongitude = (Math.PI * (double)point.Longitude) / 180;

            double newLatitude = Math.Asin(Math.Sin((double)oldLatitide) * Math.Cos(δ) + Math.Cos(angle) * Math.Sin(δ) * Math.Cos(angle));
            double newLongitude = (double)oldLongitude + Math.Atan2(Math.Sin(angle) * Math.Sin(δ) * Math.Cos((double)oldLatitide), Math.Cos(δ) - Math.Sin((double)oldLatitide) * Math.Sin(newLatitude));

            newLatitude = (180 * newLatitude) / Math.PI;
            newLongitude = (180 * newLongitude) / Math.PI;

            return new GPSLocation((decimal)newLatitude, (decimal)newLongitude);
        }

        /// <summary>
        /// Computes the polar angle between this <see cref="GPSLocation"/> and another.
        /// See <see cref="GetPolarAngle"/> for more.
        /// </summary>
        /// <param name="location">The location to include in the calculation.</param>
        /// <returns>The polar angle between the two locations.</returns>
        public double PolarAngleTo(GPSLocation location)
        {
            return GetPolarAngle(this, location);
        }
        /// <summary>
        /// Computes the polar angle between two locations.
        /// </summary>
        /// <param name="gps1">The origin.</param>
        /// <param name="gps2">The point.</param>
        /// <returns>The polar angle between <paramref name="gps1"/> and <paramref name="gps2"/>.</returns>
        public static double GetPolarAngle(GPSLocation gps1, GPSLocation gps2)
        {
            return Math.Atan2((double)(gps2.latitude - gps1.latitude), (double)(gps2.longitude - gps1.longitude));

        }

        /// <summary>
        /// Computes the distance (in meters) between this <see cref="GPSLocation"/> and another.
        /// See <see cref="GetDistance"/> for more.
        /// </summary>
        /// <param name="location">The location to include in the calculation.</param>
        /// <returns>The distance (in meters) between the two locations.</returns>
        public double DistanceTo(GPSLocation location)
        {
            return GetDistance(this, location);
        }
        /// <summary>
        /// Computes the distance (in meters) between two <see cref="GPSLocation"/>s.
        /// </summary>
        /// <param name="gps1">The first location to use in the calculation.</param>
        /// <param name="gps2">The second location to use in the calculation.</param>
        /// <returns>The distance (in meters) between the two locations.</returns>
        /// <remarks>Implemented using the description at http://www.movable-type.co.uk/scripts/latlong.html </remarks>
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

        /// <summary>
        /// Performs a grahamscan on the specified array of gpslocations
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>The set of the points that make up the convex hull</returns>
        /// <remarks>
        /// The method is implmented as a Graham Scan, as defined in:
        /// Introduction to algorithms
        /// by Cormen, Thomas H and Leiserson, Charles E and Rivest, Ronald L and Stein, Clifford and others
        /// published by MIT press Cambridge
        /// </remarks>
        public static GPSLocation[] GetConvexHull(IEnumerable<GPSLocation> data)
        {
            GPSLocation p0 = data.Aggregate((minItem, nextItem) => minItem.Longitude < nextItem.Longitude ? minItem : nextItem);
            GPSLocation[] remaining = data.Where(x => !x.Equals(p0)).Distinct().OrderBy(x => p0.PolarAngleTo(x)).ToArray();

            Stack<GPSLocation> stack = new Stack<GPSLocation>();

            if (remaining.Length >= 2)
            {
                stack.Push(p0);
                stack.Push(remaining[0]);
                stack.Push(remaining[1]);


                for (int i = 2; i < remaining.Length; i++)
                {
                    while (!isLeftTurn(stack.ElementAt(1), stack.ElementAt(0), remaining[i]))
                        stack.Pop();

                    stack.Push(remaining[i]);
                }
            }
            return stack.ToArray();
        }

        /// <summary>
        /// Determines whether the turn from the origin through p1 to p2 is a left turn 
        /// </summary>
        /// <param name="p1">The p1.</param>
        /// <param name="p2">The p2.</param>
        /// <param name="origin">The origin.</param>
        /// <returns></returns>
        private static bool isLeftTurn(GPSLocation p1, GPSLocation p2, GPSLocation origin)
        {
            GPSLocation g = p1 - origin;
            GPSLocation g2 = p2 - origin;

            return 0 > g.Latitude * g2.Longitude - g2.Latitude * g.Longitude;
        }
    }
}
