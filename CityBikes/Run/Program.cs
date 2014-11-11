using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.GeneratedDatabaseModel;

namespace Run
{
    class Program
    {
        static void Main(string[] args)
        {
            BuildMarkov bm = new BuildMarkov();
            Database database= new  Database();
            MarkovChain mc = bm.Build(database);

        }
    }
}
