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
            string[] s = FileHandler.LoadFile("bruno1.txt");
            Console.WriteLine(s.ToString());
            Console.ReadKey();
        }
    }
}
