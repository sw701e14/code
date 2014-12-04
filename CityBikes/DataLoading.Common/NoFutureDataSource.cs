using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationService.Common
{
    public class NoFutureDataSource : IDataSource
    {
        private IDataSource source;
        private GPSInput data;

        public NoFutureDataSource(IDataSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            this.source = source;
            this.data = null;
        }

        public GPSInput GetData()
        {
            if (data == null)
                data = source.GetData();

            if (data == null)
                return null;
            else if (data.Timestamp > DateTime.Now)
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
