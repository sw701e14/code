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
            Console.WriteLine(BikesNearby.GetBikesNearby());
            Console.ReadKey();
        }
    }
}
