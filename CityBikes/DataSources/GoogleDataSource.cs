using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSources
{
    public static class GoogleDataSource
    {
        private static readonly TimeSpan interval = TimeSpan.FromMinutes(5);

        public static IDataSource GetSource(Bike bike, string[] destinations, DateTime startTime, int iterations)
        {
            IEnumerable<GPSData> enumeration = GenerateGPSData.GenerateBikeRoute(bike, destinations, startTime, iterations);

            return new EnumerationDataSource(enumeration.Randomize().ConvertToInterval(interval));
        }

        public static IDataSource GetSource(IEnumerable<Bike> bikes, string[] destinations, DateTime startTime, int iterations)
        {
            return new MultiDataSource(bikes.Select(b => GetSource(b, destinations, startTime, iterations)));
        }
    }
}
