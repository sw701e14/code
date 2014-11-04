using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Library.GeneratedDatabaseModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DatabaseImportTest;
namespace Library.Tests
{
    [TestClass()]
    public class BikesNearbyTests
    {
        Database context = new Database();

        //Requires test_data.sql in the database
        [TestMethod()]
        public void GetBikeLocationTest()
        {
            GPSLocation loc = AllBikesLocation.GetBikeLocation(context, 65530);

            GPSLocation expected = new GPSLocation(4.12345678m, -4.12345678m);
            Assert.AreEqual(expected, loc);
        }

        //Requires test_data.sql in the database
        [TestMethod()]
        public void GetBikeLocationsTest()
        {
            var locations = AllBikesLocation.GetBikeLocations(context).ToArray();

            List<Tuple<long, GPSLocation>> bikesExpected = new List<Tuple<long, GPSLocation>>() {
                Tuple.Create(65530L,new GPSLocation (4.12345678m,-4.12345678m)),
                Tuple.Create(65531L,new GPSLocation (2.12345678m,-2.12345678m)),
                Tuple.Create(65532L,new GPSLocation (6.12345678m,-13.12345678m)),
                Tuple.Create(65533L,new GPSLocation (8.12345678m,-2.12345678m)),
                Tuple.Create(65534L,new GPSLocation (2.12345678m,-10.12345678m)),
                Tuple.Create(65535L,new GPSLocation (5.12345678m,-1.12345678m))
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
