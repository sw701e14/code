using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library.Clustering
{
    public class BorderPoint : Point
    {
        public BorderPoint(GPSLocation gpsLocation) : base(gpsLocation) { }
    }
}
