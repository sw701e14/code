using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library.Clustering
{
    public class Point : IEquatable<Point>
    {
        public Point(GPSLocation gpsLocation) 
        {
            this.location = gpsLocation;
        }

        private GPSLocation location;

        public GPSLocation Location
        {
            get { return location; }
        }
        
        public bool Equals(Point other)
        {
            return location.Equals(other);
        }
    }
}
