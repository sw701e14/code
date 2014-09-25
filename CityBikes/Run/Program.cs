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
            Console.WriteLine("BikesNearby:");
            foreach (var item in BikesNearby.GetBikesNearby(1,1))
            {
                Console.WriteLine("bikeID: " + item.Key + " Distance: " + item.Value.Item1 + "m" + " Date: " + item.Value.Item2);
            }

            Console.WriteLine("AvailableBikes:");

            foreach (int bikeID in AvailableBikes.GetAvailableBikes())
            {
                Console.WriteLine("bikeID: " + bikeID);
            }

            Console.ReadKey();
        }
    }
}
