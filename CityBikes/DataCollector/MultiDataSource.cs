using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    public class MultiDataSource : IDataSource
    {
        private IDataSource[] sources;
        private int index = 0;

        public MultiDataSource(IEnumerable<IDataSource> sources)
            : this(sources.ToArray())
        {
        }
        public MultiDataSource(params IDataSource[] sources)
        {
            this.sources = new IDataSource[sources.Length];
            sources.CopyTo(this.sources, 0);
        }

        public GPSData? GetData()
        {
            GPSData? data = null;

            for (int i = 0; i < sources.Length; i++)
            {
                data = sources[index].GetData();
                if (data.HasValue)
                    return data;
                else
                    next();
            }

            return null;
        }

        private void next()
        {
            index = (index + 1) % sources.Length;
        }
    }
}
