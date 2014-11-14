using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public struct GPSData
    {
        private static Random rnd = new Random();

        private Bike bike;
        private GPSLocation location;
        private byte accuracy;
        private DateTime queryTime;
        private bool hasNotMoved;

        public GPSData(Bike bike, GPSLocation location, byte? accuracy, DateTime queryTime, bool hasNotMoved)
        {
            this.bike = bike;
            this.location = location;
            this.accuracy = getAccuracy(accuracy);
            this.queryTime = queryTime;
            this.hasNotMoved = hasNotMoved;
        }

        private static byte getAccuracy(byte? accuracy)
        {
            if (!accuracy.HasValue)
                return (byte)rnd.Next(30);
            else
                return accuracy.Value;
        }

        public Bike Bike
        {
            get { return bike; }
        }
        public GPSLocation Location
        {
            get { return location; }
        }
        public byte Accuracy
        {
            get { return accuracy; }
        }
        public DateTime QueryTime
        {
            get { return queryTime; }
        }
        public bool HasNotMoved
        {
            get { return hasNotMoved; }
        }
    }
}
