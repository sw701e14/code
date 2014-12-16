using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public struct Matrix
    {
        private double[,] data;

        public Matrix(double[,] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.GetLength(0) <= 0)
                throw new ArgumentOutOfRangeException("data", "The size of a matrix must be greater than zero. It cannot be constructed from an empty array.");
            if (data.GetLength(1) <= 0)
                throw new ArgumentOutOfRangeException("data", "The size of a matrix must be greater than zero. It cannot be constructed from an empty array.");

            this.data = (double[,])data.Clone();
        }

        public double this[int x, int y]
        {
            get { return data[x, y]; }
        }
        public int Width
        {
            get { return data.GetLength(0); }
        }
        public int Height
        {
            get { return data.GetLength(1); }
        }

        public double[,] ToArray()
        {
            return (double[,])data.Clone();
        }

        public static Matrix operator *(Matrix left, Matrix right)
        {
            if (left.Height != right.Width || left.Width != right.Height)
                throw new ArgumentException("Matrices cannot be multiplied. MxN matrices can only be multiplied by NxM.");

            int w = left.Height, h = right.Width;
            double[,] result = new double[w, h];

            for (int i = 0; i < w; i++)
                for (int j = 0; j < h; j++)
                    result[i, j] = dotProduct(left.data, right.data, i, j);

            return new Matrix(result);
        }
        public static Vector operator *(Matrix matrix, Vector vector)
        {
            double[] result = new double[matrix.Width];
            for (int i = 0; i < matrix.Width; i++)
            {
                double k = 0;
                for (int j = 0; j < vector.Length; j++)
                    k += vector[j] * matrix[i, j];

                result[i] = k;
            }
            return new Vector(result);
        }
        public static Vector operator *(Vector vector, Matrix matrix)
        {
            return matrix * vector;
        }

        private static double dotProduct(double[,] left, double[,] right, int i, int j)
        {
            double sum = 0;
            for (int k = 0; k < left.GetLength(0); k++)
            {
                sum += left[i, k] * right[k, j];
            }
            return sum;
        }

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();

            for (int i = 0; i < Width; i++)
            {
                //b.Append("\\hline ");
                for (int j = 0; j < Height; j++)
                {
                    if (data[j, i] == 0)
                        b.Append(" ");
                    else
                    {
                        string s = String.Format("{0,5:0.0}", data[j, i]*100);
                        b.Append(s);
                    }
                    if (j != Width - 1)
                        b.Append(" & ");
                }
                b.Append("\\\\");
                b.Append('\n');
            }

            return b.ToString();
        }
    }
}
