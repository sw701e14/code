using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DAL;
using Shared.DTO;

namespace Library.Tests
{
    [TestClass]
    public class BikesNearbyTests
    {
        private Database database;

        [TestInitialize()]
        public void Initialize()
        {
            database = new Database();
            LibraryTests.DatabaseLoader.EnsureDB();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            database.Dispose();
            database = null;
        }

        //Requires test_data.sql in the database
        [TestMethod]
        public void GetBikeLocationTest()
        {
            GPSLocation loc = database.RunSession(s => s.GetBikeLocation(65530));

            GPSLocation expected = new GPSLocation(4.12345678m, -4.12345678m);
            Assert.AreEqual(expected, loc);
        }

        //Requires test_data.sql in the database
        [TestMethod]
        public void GetBikeLocationsTest()
        {
            var locations = database.RunSession(s => s.GetBikeLocations()).ToList();

            List<Tuple<Bike, GPSLocation>> bikesExpected = new List<Tuple<Bike, GPSLocation>>() {
                Tuple.Create(new Bike(65530),new GPSLocation (4.12345678m,-4.12345678m)),
                Tuple.Create(new Bike(65531),new GPSLocation (2.12345678m,-2.12345678m)),
                Tuple.Create(new Bike(65532),new GPSLocation (6.12345678m,-13.12345678m)),
                Tuple.Create(new Bike(65533),new GPSLocation (8.12345678m,-2.12345678m)),
                Tuple.Create(new Bike(65534),new GPSLocation (2.12345678m,-10.12345678m)),
                Tuple.Create(new Bike(65535),new GPSLocation (5.12345678m,-1.12345678m))
            };

            if (locations.Count != bikesExpected.Count)
                Assert.Fail("Number of bikes in query differs from expected.");

            foreach (var item in bikesExpected)
            {
                if (!locations.Contains(item))
                {
                    Assert.Fail("{0} is not in the resulting list", item);
                }
            }

            Assert.IsTrue(true);
        }
    }
}
