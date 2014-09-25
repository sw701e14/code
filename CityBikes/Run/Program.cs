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
                Console.WriteLine("bikeID: " + item.Key + " Distance to given point: " + item.Value.Item1 + " Date: " + item.Value.Item2);
            }
            Console.ReadKey();
        }
    }
}
