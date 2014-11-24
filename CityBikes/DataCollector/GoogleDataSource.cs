using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    public class GoogleDataSource : IDataSource
    {
        private Bike bike;

        public GoogleDataSource(Bike bike)
        {
            this.bike = bike;
        }

        public GPSData? GetData()
        {
            throw new NotImplementedException();
        }
    }
}
