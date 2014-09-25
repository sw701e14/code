using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseImportTest;
namespace Library.Tests
{
    [TestClass()]
    public class BikesNearbyTests
    {
        decimal fromLatitude = 4m;
        decimal fromLongitude = 1m;
        decimal toLatitude = 0m;
        decimal toLongitude = 1m;

        [TestMethod()]
        public void GetBikesNearbyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getDistance()
        {
            decimal actual = typeof(BikesNearby).InvokeStaticPrivate<decimal>("getDistance", fromLatitude, fromLongitude, toLatitude, toLongitude);
            decimal expected = 4m;
            Assert.AreEqual(expected, actual);
        }
    }
}
