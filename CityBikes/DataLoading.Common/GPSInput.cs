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
