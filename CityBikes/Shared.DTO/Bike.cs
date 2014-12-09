using Shared.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public struct Bike : IEquatable<Bike>
    {
        private uint id;

        public Bike(uint id)
        {
            this.id = id;
        }

        public uint Id
        {
            get { return id; }
        }

        public static GPSData[] GetLatestData(DatabaseSession session)
        {
            return session.GetLastGPSData().Select(x => new GPSData(new Bike(x.Item1), new GPSLocation(x.Item2, x.Item3), x.Item4, x.Item5, x.Item6)).ToArray();
        }
        public static GPSData? GetLatestData(DatabaseSession session, Bike bike)
        {
            var t = session.GetLastGPSData(bike.id);
            if (t == null)
                return null;
            else
                return new GPSData(bike, new GPSLocation(t.Item1, t.Item2), t.Item3, t.Item4, t.Item5);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Bike))
                return false;
            else
                return Equals((Bike)obj);
        }
        public bool Equals(Bike other)
        {
            return this.id.Equals(other.id);
        }

        public static bool operator ==(Bike b1, Bike b2)
        {
            return b1.Equals(b2);
        }
        public static bool operator !=(Bike b1, Bike b2)
        {
            return !b1.Equals(b2);
        }

        public override int GetHashCode()
        {
            return (int)id;
        }

    }
}
