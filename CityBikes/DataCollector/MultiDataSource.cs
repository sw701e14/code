using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataCollector
{
    /// <summary>
    /// Retrives <see cref="GPSData"/> from a collection of data sources by querying each of them one at a time.
    /// </summary>
    public class MultiDataSource : IDataSource
    {
        private IDataSource[] sources;
        private int index = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDataSource"/> class.
        /// </summary>
        /// <param name="sources">A collection of the sources that should be queried by this <see cref="MultiDataSource"/>.</param>
        public MultiDataSource(IEnumerable<IDataSource> sources)
            : this(sources.ToArray())
        {
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDataSource"/> class.
        /// </summary>
        /// <param name="sources">An array containing the sources that should be queried by this <see cref="MultiDataSource"/>.</param>
        public MultiDataSource(params IDataSource[] sources)
        {
            this.sources = new IDataSource[sources.Length];
            sources.CopyTo(this.sources, 0);
        }

        /// <summary>
        /// Gets data from one of the data sources managed by this <see cref="MultiDataSource"/>, if available.
        /// </summary>
        /// <returns>
        /// The oldest <see cref="GPSData" /> available at the data source; <c>null</c> if no data is available.
        /// </returns>
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
