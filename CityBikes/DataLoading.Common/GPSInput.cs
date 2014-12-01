using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoading.Common
{
    public struct GPSInput
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
        private DateTime queryTime;

        public GPSInput(uint bikeId, decimal latitude, decimal longitude, byte? accuracy, DateTime queryTime)
        {
            this.bikeId = bikeId;
            this.latitude = latitude;
            this.longitude = longitude;
            this.accuracy = getAccuracy(accuracy);
            this.queryTime = queryTime;
        }

        public uint BikeId
        {
            get { return bikeId; }
        }
        public decimal Latitude
        {
            get { return latitude; }
        }
        public decimal Longitude
        {
            get { return longitude; }
        }
        public byte Accuracy
        {
            get { return accuracy; }
        }
        public DateTime QueryTime
        {
            get { return queryTime; }
        }
    }
}
