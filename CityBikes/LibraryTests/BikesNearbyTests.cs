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
            AllBikesLocation abl = new AllBikesLocation();
            var locations = abl.GetBikeLocations().ToArray();

            List<Tuple<int, GPSLocation>> bikesExpected = new List<Tuple<int, GPSLocation>>() {
                Tuple.Create(65530,new GPSLocation (4.12345678m,-4.12345678m)),
                Tuple.Create(65531,new GPSLocation (2.12345678m,-2.12345678m)),
                Tuple.Create(65532,new GPSLocation (6.12345678m,-13.12345678m)),
                Tuple.Create(65533,new GPSLocation (8.12345678m,-2.12345678m)),
                Tuple.Create(65534,new GPSLocation (2.12345678m,-10.12345678m)),
                Tuple.Create(65535,new GPSLocation (5.12345678m,-1.12345678m))
            };

            

            if (locations.Count() != bikesExpected.Count)
                Assert.Fail();

            foreach (var item in bikesExpected)
            {
                if (!locations.Contains(item))
                {
                    Assert.Fail("{0} is not in the resulting list");
                }
            }

            Assert.IsTrue(true);
        }
    }
}
