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
        private int size;

        public Matrix(double[,] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.GetLength(0) <= 0)
                throw new ArgumentOutOfRangeException("data", "The size of a matrix must be greater than zero. It cannot be constructed from an empty array.");
            if (data.GetLength(1) <= 0)
                throw new ArgumentOutOfRangeException("data", "The size of a matrix must be greater than zero. It cannot be constructed from an empty array.");
            if (data.GetLength(0) != data.GetLength(0))
                throw new ArgumentOutOfRangeException("data", "Matrix can only be used to represent square matrices.");

            this.size = data.GetLength(0);

            this.data = new double[size, size];
            data.CopyTo(this.data, 0);
        }
    }
}
