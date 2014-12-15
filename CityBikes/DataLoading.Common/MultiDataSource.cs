using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocationService.Common
{
    /// <summary>
    /// Retrives <see cref="GPSInput"/> from a collection of data sources by querying each of them one at a time.
    /// </summary>
    public class MultiDataSource : IDataSource
    {
        private Tuple<IDataSource, GPSInput>[] sources;

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
            this.sources = new Tuple<IDataSource, GPSInput>[sources.Length];
            for (int i = 0; i < sources.Length; i++)
                this.sources[i] = new Tuple<IDataSource, GPSInput>(sources[i], sources[i].GetData());
        }

        /// <summary>
        /// Gets data from one of the data sources managed by this <see cref="MultiDataSource"/>, if available.
        /// </summary>
        /// <returns>
        /// The oldest <see cref="GPSData" /> available at the data source; <c>null</c> if no data is available.
        /// </returns>
        public GPSInput GetData()
        {
            Array.Sort(sources, compare);

            var s = sources[0];
            sources[0] = new Tuple<IDataSource, GPSInput>(s.Item1, s.Item1.GetData());

            return s.Item2;
        }

        private int compare(Tuple<IDataSource, GPSInput> a, Tuple<IDataSource, GPSInput> b)
        {
            if (a.Item2 == null && b.Item2 == null)
                return 0;
            else if (a.Item2 == null)
                return 1;
            else if (b.Item2 == null)
                return -1;
            else
                return a.Item2.Timestamp.CompareTo(b.Item2.Timestamp);
        }
    }
}
