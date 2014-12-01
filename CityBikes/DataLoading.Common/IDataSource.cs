using Library;

namespace DataLoading.Common
{
    /// <summary>
    /// Defines methods used to retrieve <see cref="GPSData"/> from a data source.
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Gets data from the data source, if available.
        /// </summary>
        /// <returns>The oldest <see cref="GPSData"/> available at the data source; <c>null</c> if no data is available.</returns>
        GPSData? GetData();
    }
}
