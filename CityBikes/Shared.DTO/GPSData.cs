using Shared.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public struct GPSData : IEquatable<GPSData>
    {
        private static Random rnd = new Random();

        private Bike bike;
        private GPSLocation location;
        private byte accuracy;
        private DateTime queryTime;
        private bool hasNotMoved;

        public GPSData(Bike bike, GPSLocation location, byte? accuracy, DateTime queryTime, bool hasNotMoved = false)
        {
            this.bike = bike;
            this.location = location;
            this.accuracy = getAccuracy(accuracy);
            this.queryTime = queryTime;
            this.hasNotMoved = false;
        }

        public static void InsertInDatabase(DatabaseSession session, GPSData data)
        {
            if (data.hasNotMoved)
                throw new ArgumentException("Data cannot be inserted with hasNotMoved = true");

            session.InsertGPSData(data.bike.Id, data.location.Latitude, data.location.Longitude, data.accuracy, data.queryTime);
        }

        public static GPSData[] GetAll(DatabaseSession session)
        {
            return session.GetAllGPSData().Select(x => new GPSData(new Bike(x.Item1), new GPSLocation(x.Item2, x.Item3), x.Item4, x.Item5, x.Item6)).ToArray();
        }
        public static GPSData[] GetAllHasNotMoved(DatabaseSession session)
        {
            return session.GetAllGPSNotMovedData().Select(x => new GPSData(new Bike(x.Item1), new GPSLocation(x.Item2, x.Item3), x.Item4, x.Item5, x.Item6)).ToArray();
        }
        public static GPSData[] GetAllHasMoved(DatabaseSession session)
        {
            return session.GetAllGPSHasMovedData().Select(x => new GPSData(new Bike(x.Item1), new GPSLocation(x.Item2, x.Item3), x.Item4, x.Item5, x.Item6)).ToArray();
        }

        public static GPSData[] GetBikeData(DatabaseSession session, Bike bike, bool latestFirst)
        {
            return session.GetBikeGPSData(bike.Id, latestFirst).Select(x => new GPSData(bike, new GPSLocation(x.Item1, x.Item2), x.Item3, x.Item4, x.Item5)).ToArray();
        }

        private static byte getAccuracy(byte? accuracy)
        {
            if (!accuracy.HasValue)
                return (byte)rnd.Next(30);
            else
                return accuracy.Value;
        }

        /// <summary>
        /// Determines if two <see cref="GPSData"/> points are equal, with respect to each of their accuracies.
        /// </summary>
        /// <param name="d1">The first data-point.</param>
        /// <param name="d2">The second data-point.</param>
        /// <returns><c>true</c> if the two points are within range of their respective accuracies; otherwise <c>false</c>.</returns>
        public static bool WithinAccuracy(GPSData d1, GPSData d2)
        {
            double dist = GPSLocation.GetDistance(d1.Location, d2.Location);
            return d1.accuracy + d2.accuracy >= dist;
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

        public override bool Equals(object obj)
        {
            if (!(obj is GPSData))
                return false;
            else
                return Equals((GPSData)obj);
        }
        public bool Equals(GPSData other)
        {
            return
                bike.Equals(other.bike) &&
                location.Equals(other.location) &&
                accuracy.Equals(other.accuracy) &&
                queryTime.Equals(other.queryTime) &&
                hasNotMoved.Equals(other.hasNotMoved);
        }
    }
}
