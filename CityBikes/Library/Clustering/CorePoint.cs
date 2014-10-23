using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library.Clustering
{
    public class CorePoint : Point
    {
        public CorePoint(GPSLocation gpsLocation) : base(gpsLocation) { }

        public class PointCollection
        {
            private List<Point> vicinityPoints;
            public PointCollection()
            {
                this.vicinityPoints = new List<Point>();
            }
        }
    }
}
