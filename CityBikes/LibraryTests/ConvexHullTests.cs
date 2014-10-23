using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.GeneratedDatabaseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace Library.Tests
{
    [TestClass()]
    public class ConvexHullTests
    {
        GPSLocation[] locations;
        
        [TestInitialize]
        public void setup()
        {
            locations = new GPSLocation[] { new GPSLocation(1, 1), new GPSLocation(10, 1), new GPSLocation(10, 10), new GPSLocation(1, 10), new GPSLocation(5, 5), new GPSLocation(3, 7), new GPSLocation(4, 2), new GPSLocation(7, 3), new GPSLocation(15, 5), new GPSLocation(5,15) , new GPSLocation(10,1)};
        }

        [TestMethod()]
        public void GrahamScanTest()
        {
            GPSLocation[] actual = ConvexHull.GrahamScan(locations);

            GPSLocation[] expected = new GPSLocation[] { new GPSLocation(1, 1), new GPSLocation(1, 10), new GPSLocation(15, 5), new GPSLocation(10, 1) ,new GPSLocation(5,15)};

            Assert.AreEqual(actual.Count(), expected.Count());

            foreach (var expectedItem in expected)
            {
                if(!actual.Contains(expectedItem))
                    Assert.Fail("Actual does not contain the expected item");
            }
        }
    }
}
