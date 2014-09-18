using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Library.Tests
{
    [TestClass()]
    public class CSVParserTests
    {
        GPSPoint gps1;
        GPSPoint gps2;
        GPSPoint gps3;
        GPSPoint[] data = new GPSPoint[1];

        [TestInitialize]
        public void Setup()
        {
            gps1 = new GPSPoint(new DateTime(2014, 9, 12, 11, 34, 03), 57.01162, 9.99120, 49, 1);
            gps2 = new GPSPoint(new DateTime(2014, 9, 12, 01, 54, 02), 57.01194, 9.99110, 16, 1);
            gps3 = new GPSPoint(new DateTime(2014, 9, 12, 02, 14, 02), 57.02801, 9.94940, 16, 1);
            data[0] = gps1;
        }

        [TestMethod()]
        public void GetDataTestTimeStamp()
        {
            GPSPoint[] actual = CSVParser.GetData("test.txt", 1);
            GPSPoint[] expected = data;
            
            Assert.AreEqual(expected[0].TimeStamp, actual[0].TimeStamp);
        }

        [TestMethod()]
        public void GetDataTestLat()
        {
            GPSPoint[] actual = CSVParser.GetData("test.txt", 1);
            GPSPoint[] expected = data;

            Assert.AreEqual(expected[0].Latitude, actual[0].Latitude);
        }

        [TestMethod()]
        public void GetDataTestLon()
        {
            GPSPoint[] actual = CSVParser.GetData("test.txt", 1);
            GPSPoint[] expected = data;

            Assert.AreEqual(expected[0].Longitude, actual[0].Longitude);
        }

        [TestMethod()]
        public void GetDataTestAcc()
        {
            GPSPoint[] actual = CSVParser.GetData("test.txt", 1);
            GPSPoint[] expected = data;

            Assert.AreEqual(expected[0].Accuracy, actual[0].Accuracy);
        }

        [TestMethod()]
        public void GetDataTestID()
        {
            GPSPoint[] actual = CSVParser.GetData("test.txt", 1);
            GPSPoint[] expected = data;

            Assert.AreEqual(expected[0].BikeId, actual[0].BikeId);
        }
    }
}
