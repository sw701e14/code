using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace DatabaseImportTest
{
    [TestClass()]
    public class SQLExportTests
    {
        GPSPoint gps1;
        GPSPoint gps2;
        GPSPoint gps3;

        [TestInitialize]
        public void Setup()
        {
            gps1 = new GPSPoint(new DateTime(2014, 1, 1, 12, 0, 0), 57.01163, 9.99110, 10, 1);
            gps2 = new GPSPoint(new DateTime(2014, 1, 1, 12, 30, 09), 57.03106, 9.94580, 30, 1);
            gps3 = new GPSPoint(new DateTime(2014, 3, 13, 23, 24, 23), 57.01271, 9.98780, null, 1);
        }

        [TestMethod()]
        public void writeGPSPointTest1()
        {
            string output = typeof(SQLExport).InvokeStaticPrivate<string>("writeGPSPoint", gps1);
            string expected = "('2014-01-01 12:00:00', 57.01163, 9.9911, 10, 1)";

            Assert.IsTrue(expected.SequenceEqual(output));
        }

        [TestMethod()]
        public void writeGPSPointTest2()
        {
            string output = typeof(SQLExport).InvokeStaticPrivate<string>("writeGPSPoint", gps2);
            string expected = "('2014-01-01 12:30:09', 57.03106, 9.9458, 30, 1)";

            Assert.IsTrue(expected.SequenceEqual(output));
        }
    }
}
