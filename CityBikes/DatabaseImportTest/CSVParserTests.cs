using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseImport;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library;

namespace DatabaseImportTest
{
    [TestClass()]
    public class CSVParserTests
    {
        Bike bike;
        GPSData gps1;
        GPSData gps2;
        GPSData[] data = new GPSData[2];

        [TestInitialize]
        public void Setup()
        {
            bike = new Bike(1);
            gps1 = new GPSData(bike, new GPSLocation(57.01162m, 9.99120m), 49, new DateTime(2014, 9, 12, 11, 34, 03), false);
            gps2 = new GPSData(bike, new GPSLocation(57.01162m, 9.99120m), 49, new DateTime(2014, 9, 12, 23, 34, 03), false);

            data[0] = gps1;
            data[1] = gps2;
        }

        [TestMethod()]
        public void GetDataTestTimeStamp()
        {
            GPSData[] actual = CSVParser.GetData("test.txt", bike);
            GPSData[] expected = data;
            
            Assert.AreEqual(expected[0].QueryTime, actual[0].QueryTime);
        }

        [TestMethod()]
        public void GetDataTestTimeStampPM()
        {
            GPSData[] actual = CSVParser.GetData("test.txt", bike);
            GPSData[] expected = data;

            Assert.AreEqual(expected[1].QueryTime, actual[1].QueryTime);
        }

        [TestMethod()]
        public void GetDataTestLat()
        {
            GPSData[] actual = CSVParser.GetData("test.txt", bike);
            GPSData[] expected = data;

            Assert.AreEqual(expected[0].Location.Latitude, actual[0].Location.Latitude);
        }

        [TestMethod()]
        public void GetDataTestLon()
        {
            GPSData[] actual = CSVParser.GetData("test.txt", bike);
            GPSData[] expected = data;

            Assert.AreEqual(expected[0].Location.Longitude, actual[0].Location.Longitude);
        }

        [TestMethod()]
        public void GetDataTestAcc()
        {
            GPSData[] actual = CSVParser.GetData("test.txt", bike);
            GPSData[] expected = data;

            Assert.AreEqual(expected[0].Accuracy, actual[0].Accuracy);
        }

        [TestMethod()]
        public void GetDataTestID()
        {
            GPSData[] actual = CSVParser.GetData("test.txt", bike);
            GPSData[] expected = data;

            Assert.AreEqual(expected[0].Bike, actual[0].Bike);
        }
    }
}
