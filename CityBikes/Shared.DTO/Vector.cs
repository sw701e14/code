using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public struct Vector
    {
        private readonly double[] data;

        public Vector(double[] data)
        {
            if (data == null)
                throw new ArgumentNullException("data");

            if (data.Length <= 0)
                throw new ArgumentOutOfRangeException("data", "The size of a vector must be greater than zero. It cannot be constructed from an empty array.");

            this.data = new double[data.Length];
            data.CopyTo(this.data, 0);
        }

        public double this[int index]
        {
            get { return data[index]; }
        }

        public int Length
        {
            get { return data.Length; }
        }
    }
}
