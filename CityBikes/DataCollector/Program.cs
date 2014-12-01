using DataLoading.Common;
using DataLoading.LocationSource;
using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DataLoading.DataCollector
{
    class Program
    {
        private static IDataSource createDataSource()
        {
            Console.Write("Specify number of bikes: ");
            string numberStr = Console.ReadLine();
            int count = int.Parse(numberStr);
            Console.WriteLine("Data will be generated for {0} bikes.", count);
            Console.WriteLine();

            List<Bike> bikes = new List<Bike>();
            for (uint i = 10; i < 10 + count; i++)
                bikes.Add(new Bike(i));

            DateTime start = DateTime.Now.Date.AddDays(-1);

            return new MultiDataSource(bikes.Select(b => new NoFutureDataSource(GoogleDataSource.GetSource(b.Id, start, int.MaxValue))));
        }

        static void Main(string[] args)
        {
            DataLoader.StartLoad(createDataSource(), true);
        }
    }
}
