using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseImport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library.GeneratedDatabaseModel;

namespace DatabaseImportTest
{
    [TestClass()]
    public class CSVParserTests
    {
        gps_data gps1;
        gps_data gps2;
        gps_data[] data = new gps_data[2];

        [TestInitialize]
        public void Setup()
        {
            gps1 = new gps_data(new DateTime(2014, 9, 12, 11, 34, 03), 57.01162m, 9.99120m, 49, 1);
            gps2 = new gps_data(new DateTime(2014, 9, 12, 23, 34, 03), 57.01162m, 9.99120m, 49, 1);
            data[0] = gps1;
            data[1] = gps2;
        }

        [TestMethod()]
        public void GetDataTestTimeStamp()
        {
            gps_data[] actual = CSVParser.GetData("test.txt", 1);
            gps_data[] expected = data;
            
            Assert.AreEqual(expected[0].queried, actual[0].queried);
        }

        [TestMethod()]
        public void GetDataTestTimeStampPM()
        {
            gps_data[] actual = CSVParser.GetData("test.txt", 1);
            gps_data[] expected = data;

            Assert.AreEqual(expected[1].queried, actual[1].queried);
        }

        [TestMethod()]
        public void GetDataTestLat()
        {
            gps_data[] actual = CSVParser.GetData("test.txt", 1);
            gps_data[] expected = data;

            Assert.AreEqual(expected[0].latitude, actual[0].latitude);
        }

        [TestMethod()]
        public void GetDataTestLon()
        {
            gps_data[] actual = CSVParser.GetData("test.txt", 1);
            gps_data[] expected = data;

            Assert.AreEqual(expected[0].longitude, actual[0].longitude);
        }

        [TestMethod()]
        public void GetDataTestAcc()
        {
            gps_data[] actual = CSVParser.GetData("test.txt", 1);
            gps_data[] expected = data;

            Assert.AreEqual(expected[0].accuracy, actual[0].accuracy);
        }

        [TestMethod()]
        public void GetDataTestID()
        {
            gps_data[] actual = CSVParser.GetData("test.txt", 1);
            gps_data[] expected = data;

            Assert.AreEqual(expected[0].bikeId, actual[0].bikeId);
        }
    }
}
