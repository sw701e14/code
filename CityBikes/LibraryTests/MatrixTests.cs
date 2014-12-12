using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryTests
{
    [TestClass]
    public class MatrixTests
    {
        private Matrix matrix;
        private Vector vector;

        private Matrix probMatrix;
        private Vector probVector;

        [TestInitialize()]
        public void Initialize()
        {
            double[,] mValues = new double[3, 3];
            mValues[0, 0] = 3;
            mValues[1, 0] = 4;
            mValues[2, 0] = 2;

            mValues[0, 1] = 2;
            mValues[1, 1] = 5;
            mValues[2, 1] = 3;

            mValues[0, 2] = 3;
            mValues[1, 2] = 1;
            mValues[2, 2] = 1;
            matrix = new Matrix(mValues);

            double[] vValues = new double[3];
            vValues[0] = 1;
            vValues[1] = 2;
            vValues[2] = 1;
            vector = new Vector(vValues);


            mValues = new double[2, 2];
            // Probabilities are entered using mValues[to, from]
            //From state A
            mValues[0, 0] = 0.2;
            mValues[1, 0] = 0.8;

            //From state B
            mValues[0, 1] = 0.4;
            mValues[1, 1] = 0.6;
            probMatrix = new Matrix(mValues);

            vValues = new double[2];
            vValues[0] = 5;
            vValues[1] = 2;
            probVector = new Vector(vValues);
        }

        [TestMethod]
        public void MatrixMultiplicationTest1()
        {
            var m2 = matrix * matrix;

            Assert.AreEqual(23, m2[0, 0]);
            Assert.AreEqual(34, m2[1, 0]);
            Assert.AreEqual(20, m2[2, 0]);

            Assert.AreEqual(25, m2[0, 1]);
            Assert.AreEqual(36, m2[1, 1]);
            Assert.AreEqual(22, m2[2, 1]);

            Assert.AreEqual(14, m2[0, 2]);
            Assert.AreEqual(18, m2[1, 2]);
            Assert.AreEqual(10, m2[2, 2]);
        }

        [TestMethod]
        public void MatrixVectorMultiplicationTest1()
        {
            var v2 = matrix * vector;

            Assert.AreEqual(10, v2[0]);
            Assert.AreEqual(15, v2[1]);
            Assert.AreEqual(9, v2[2]);
        }

        [TestMethod]
        public void MatrixVectorMultiplicationTest2()
        {
            var v2 = (vector * matrix) * matrix;
            var v3 = vector * (matrix * matrix);

            Assert.AreEqual(v2[0], v3[0]);
            Assert.AreEqual(v2[1], v3[1]);
            Assert.AreEqual(v2[2], v3[2]);
        }

        [TestMethod]
        public void ProbabilityPrediction()
        {
            Vector v1 = probMatrix * probVector;

            Assert.AreEqual(1.8, v1[0]);
            Assert.AreEqual(5.2, v1[1]);
        }
    }
}
