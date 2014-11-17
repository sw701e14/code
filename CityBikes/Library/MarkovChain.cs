using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class MarkovChain
    {
        private double[,] markovChain;

        public double this[int i, int j]
        {
            get { return markovChain[i, j]; }
            set { markovChain[i, j] = value; }
        }

        public MarkovChain(int size)
        {
            markovChain = new double[size,size];
        }
    }
}
