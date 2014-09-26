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
            foreach (var item in BikesNearby.GetBikesNearby(1,1))
            {
                Console.WriteLine("bikeID: " + item.Item1 + " Distance to given point: " + item.Item2);
            }
            Console.ReadKey();
        }
    }
}
