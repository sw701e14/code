namespace LocationService.Common
{
    /// <summary>
    /// Defines methods used to retrieve <see cref="GPSInput"/> from a data source.
    /// </summary>
    public interface IDataSource
    {
        /// <summary>
        /// Gets data from the data source, if available.
        /// </summary>
        /// <returns>The oldest <see cref="GPSInput"/> available at the data source; <c>null</c> if no data is available.</returns>
        GPSInput GetData();
    }
}
