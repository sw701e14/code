using Library;
using System;
using System.Collections.Generic;

namespace DataSources
{
    public class EnumerationDataSource : IDataSource
    {
        private IEnumerable<GPSData> data;
        private IEnumerator<GPSData> e;

        public EnumerationDataSource(IEnumerable<GPSData> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.data = data;
            this.e = this.data.GetEnumerator();
        }

        public GPSData? GetData()
        {
            if (e.MoveNext())
                return e.Current;
            else
            {
                e.Dispose();
                return null;
            }
        }
    }
}
