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
    public class BikesNearbyTests
    {
        Database context = new Database();

        [TestMethod()]
        public void GetBikesNearbyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetBikeLocationTest()
        {
            AllBikesLocation abl = new AllBikesLocation();
            GPSLocation loc = abl.GetBikeLocation(65530);

            GPSLocation expected = new GPSLocation(4.12345678m,-4.12345678m);
            Assert.AreEqual(expected, loc);
        }

        [TestMethod()]
        public void GetBikeLocationsTest()
        {
            Assert.Fail();
        }
    }
}
