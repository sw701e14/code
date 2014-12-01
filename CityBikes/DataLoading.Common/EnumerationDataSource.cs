using System;
using System.Collections.Generic;

namespace DataLoading.Common
{
    public class EnumerationDataSource : IDataSource
    {
        private IEnumerator<GPSInput> e;

        public EnumerationDataSource(IEnumerable<GPSInput> data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            this.e = data.GetEnumerator();
        }

        public GPSInput GetData()
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
