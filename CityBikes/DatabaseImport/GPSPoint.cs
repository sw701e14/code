using System;

namespace DatabaseImport
{
    /// <summary>
    /// Describes the coordinates of a GPS-measurement (a point).
    /// </summary>
    public struct GPSPoint : IEquatable<GPSPoint>
    {
        private readonly DateTime timestamp;
        private readonly double latitude, longitude;
        private readonly int? accuracy;
        private readonly int bikeId;

        /// <summary>
        /// Gets the timestamp when the measurement was recorded.
        /// </summary>
        public DateTime TimeStamp
        {
            get { return timestamp; }
        }
        /// <summary>
        /// Gets the latitude associated with this <see cref="GPSPoint"/>.
        /// </summary>
        public double Latitude
        {
            get { return latitude; }
        }
        /// <summary>
        /// Gets the longitude associated with this <see cref="GPSPoint"/>.
        /// </summary>
        public double Longitude
        {
            get { return longitude; }
        }
        /// <summary>
        /// Gets the accuracy of the measurement, or <c>null</c> if no accuracy was recorded.
        /// </summary>
        public int? Accuracy
        {
            get { return accuracy; }
        }
        /// <summary>
        /// Gets the identifier for the bike to which this measurement belongs.
        /// </summary>
        public int BikeId
        {
            get { return bikeId; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GPSPoint"/> struct.
        /// </summary>
        /// <param name="timestamp">The timestamp when the measurement was recorded.</param>
        /// <param name="latitude">The latitude associated with this <see cref="GPSPoint"/>.</param>
        /// <param name="longitude">The longitude associated with this <see cref="GPSPoint"/>.</param>
        /// <param name="accuracy">The accuracy of the measurement, or <c>null</c> if no accuracy was recorded.</param>
        /// <param name="bikeId">The identifier for the bike to which this measurement belongs.</param>
        public GPSPoint(DateTime timestamp, double latitude, double longitude, int? accuracy, int bikeId)
        {
            if (latitude > 90 || latitude < -90)
                throw new ArgumentOutOfRangeException("latitude");

            if (longitude > 180 || longitude < -180)
                throw new ArgumentOutOfRangeException("longitude");

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

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return timestamp.GetHashCode() ^ latitude.GetHashCode() ^ longitude.GetHashCode() ^ accuracy.GetHashCode() ^ bikeId.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance of <see cref="GPSPoint"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is GPSPoint)
                return Equals((GPSPoint)obj);
            else
                return false;
        }

        /// <summary>
        /// Indicates whether the current <see cref="GPSPoint"/> is equal to another <see cref="GPSPoint"/>.
        /// </summary>
        /// <param name="other">A <see cref="GPSPoint"/> to compare with this <see cref="GPSPoint"/>.</param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
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
