using Library;
using System;
using System.Collections.Generic;

namespace DataLoading.Common
{
    public class EnumerationDataSource : IDataSource
    {
        private IEnumerator<GPSData> e;

        public EnumerationDataSource(IEnumerable<GPSData> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.e = data.GetEnumerator();
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
