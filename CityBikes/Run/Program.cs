using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            AllBikesLocation abl = new AllBikesLocation();
            foreach (var item in abl.GetBikeLocations())
            {
                Console.WriteLine(item.bikeId);
            }

            Console.ReadKey();
        }
    }
}
