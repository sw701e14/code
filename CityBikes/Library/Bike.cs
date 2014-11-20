using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
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

        public GPSData LatestGPSData(Database.DatabaseSession session)
        {
            return session.ExecuteRead("SELECT * FROM citybike_test.gps_data WHERE bikeId = {0} ORDER BY queried DESC", id).First().GetGPSData();
        }
    }
}
