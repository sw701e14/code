using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class MarkovChain
    {
        double[,] markovChain;
        private double[,] markovFrequencies;

        public double this[int i, int j]
        {
            get { return markovFrequencies[i, j]; }
            set { markovFrequencies[i, j] = value; }
        }

        public MarkovChain(int size)
        {
            markovFrequencies = new double[size, size];
            markovChain = new double[size, size];
        }

        private int Sum()
        {
            int k = 0;
            foreach (var item in markovFrequencies)
            {
                k += (int)item;
            }
            return k;
        }

        public int size
        {
            get { return markovChain.Length; }
        }
        


        /// <summary>
        /// Creates the markov chain from the frequencies inputted and saves it in the MarkovChain
        /// </summary>
        public void CreateChain()
        {
            int s = Sum();

            for (int i = 0; i < markovFrequencies.GetLength(0); i++)
            {
                for (int j = 0; j < markovFrequencies.GetLength(1); j++)
                {
                    markovChain[i, j] = markovFrequencies[i, j] / s;
                }
            }
        }

        /// <summary>
        /// Calculates the matrix Pn for the specified n value.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns>The Pn matrix</returns>
        public double[,] Pn(int n)
        {
            double[,] current = new double[markovChain.GetLength(0), markovChain.GetLength(0)];
            markovChain.CopyTo(current, 0);
            for (int i = 0; i < n - 1; i++)
            {
                current = matrixMultiplication(current, markovChain);
            }

            return current;
        }

        /// <summary>
        /// Multiplies two matrices together
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>A matrix containing the product of the two matrices</returns>
        private double[,] matrixMultiplication(double[,] left, double[,] right)
        {
            double[,] result = new double[left.GetLength(1), right.GetLength(0)];

            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    result[i, j] = dotProduct(left, right, i, j);
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the product of a matrix and a vector 
        /// </summary>
        /// <param name="matrix">The matrix.</param>
        /// <param name="vector">The vector.</param>
        /// <returns>A vector with the product of the matrxi and the vector</returns>
        public double[] matrixVectorProduct(double[,] matrix, double[] vector)
        {
            double[] result = new double[matrix.GetLength(0)];
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                double k = 0;
                for (int j = 0; j < vector.Length; j++)
                {

                    k += vector[i] * matrix[i, j];
                }
                result[i] = k;
            }
            return result;
        }

        private double dotProduct(double[,] left, double[,] right, int row, int column)
        {
            double k = 0;
            for (int i = 0; i < left.GetLength(0); i++)
            {
                k += left[i, row] * right[column, i];
            }
            return k;
        }



    }
}
