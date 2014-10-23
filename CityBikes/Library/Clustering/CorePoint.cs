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
        public CorePoint(GPSLocation gpsLocation, List<Point> neighborhood) : base(gpsLocation) 
        {
            this.neighborhood = new PointCollection(neighborhood);     
        }

        private PointCollection neighborhood;

        public PointCollection Neighborhood
        {
            get { return neighborhood; }
        }
        

        public class PointCollection : IEnumerable<Point>
        {
            private List<Point> vicinityPoints;
            public PointCollection(List<Point> points)
            {
                this.vicinityPoints = points;
            }

            public IEnumerator<Point> GetEnumerator()
            {
                return this.vicinityPoints.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.vicinityPoints.GetEnumerator();
            }
            public void Add(Point point)
            {
                this.vicinityPoints.Add(point);
            }

        }
    }
}
