using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoading.LocationSource
{
    public class NoFutureDataSource : IDataSource
    {
        private IDataSource source;
        private GPSData? data;

        public NoFutureDataSource(IDataSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.source = source;
            this.data = null;
        }

        public GPSData? GetData()
        {
            if (!data.HasValue)
                data = source.GetData();

            if (!data.HasValue)
                return null;
            else if (data.Value.QueryTime > DateTime.Now)
                return null;
            else
            {
                var d = data;
                data = null;
                return d;
            }
        }
    }
}
