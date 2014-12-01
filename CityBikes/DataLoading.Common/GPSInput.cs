using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoading.Common
{
    /// <summary>
    /// Represents data that is retrieved from a GPS unit.
    /// </summary>
    public class GPSInput
    {
        private static Random rnd = new Random();
        private static byte getAccuracy(byte? accuracy)
        {
            if (!accuracy.HasValue)
                return (byte)rnd.Next(30);
            else
                return accuracy.Value;
        }

        private uint bikeId;
        private decimal latitude, longitude;
        private byte accuracy;
        private DateTime timestamp;

        /// <summary>
        /// Initializes a new instance of the <see cref="GPSInput"/> class.
        /// </summary>
        /// <param name="bikeId">The identifier for the bike that this GPS data is associated with.</param>
        /// <param name="latitude">The latitude component of the GPS data.</param>
        /// <param name="longitude">The longitude component of the GPS data.</param>
        /// <param name="accuracy">The accuracy of the GPS measurement (in meters). Setting this value to <c>null</c> will result in a random accuracy.</param>
        /// <param name="timestamp">The time at which the GPS data was recorded.</param>
        public GPSInput(uint bikeId, decimal latitude, decimal longitude, byte? accuracy, DateTime timestamp)
        {
            this.bikeId = bikeId;
            this.latitude = latitude;
            this.longitude = longitude;
            this.accuracy = getAccuracy(accuracy);
            this.timestamp = timestamp;
        }

        /// <summary>
        /// Adds random the noise (within accuracy) to a <see cref="GPSInput"/> node.
        /// Applying this method to the same instance multiple times could result in errors.
        /// </summary>
        public void AddNoise()
        {
            double angle = rnd.NextDouble() * 2 * Math.PI;
            double distance = rnd.NextDouble() * (double)accuracy;
            distance /= 1000.0;

            int R = 6371; // earth radius in km
            double δ = distance / R;

            double oldLatitide = (Math.PI * (double)latitude) / 180;
            double oldLongitude = (Math.PI * (double)longitude) / 180;

            double newLatitude = Math.Asin(Math.Sin((double)oldLatitide) * Math.Cos(δ) + Math.Cos(angle) * Math.Sin(δ) * Math.Cos(angle));
            double newLongitude = (double)oldLongitude + Math.Atan2(Math.Sin(angle) * Math.Sin(δ) * Math.Cos((double)oldLatitide), Math.Cos(δ) - Math.Sin((double)oldLatitide) * Math.Sin(newLatitude));

            newLatitude = (180 * newLatitude) / Math.PI;
            newLongitude = (180 * newLongitude) / Math.PI;

            this.latitude = (decimal)newLatitude;
            this.longitude = (decimal)newLongitude;
        }

        /// <summary>
        /// Gets the identifier for the bike that this GPS data is associated with.
        /// </summary>
        public uint BikeId
        {
            get { return bikeId; }
        }
        /// <summary>
        /// Gets the latitude component of the GPS data.
        /// </summary>
        public decimal Latitude
        {
            get { return latitude; }
        }
        /// <summary>
        /// Gets the longitude component of the GPS data.
        /// </summary>
        public decimal Longitude
        {
            get { return longitude; }
        }
        /// <summary>
        /// Gets the accuracy of the GPS measurement (in meters).
        /// </summary>
        public byte Accuracy
        {
            get { return accuracy; }
        }
        /// <summary>
        /// Gets the time at which the GPS data was recorded.
        /// </summary>
        public DateTime Timestamp
        {
            get { return timestamp; }
        }
    }
}
