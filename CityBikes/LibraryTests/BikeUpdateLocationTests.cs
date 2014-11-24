using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library;

namespace LibraryTests
{
    [TestClass]
    public class BikeUpdateLocationTests
    {
        private Database database;
        int rndNumber = 50;

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

        /// <summary>
        /// Make sure the expected <see cref="GPSData"/> is not already in database, else the test will fail.
        /// </summary>
        [TestMethod]
        public void addNewLocation()
        {
            var bike = new Bike((uint)rndNumber);
            GPSData expected = new GPSData(bike, new GPSLocation(rndNumber, rndNumber), (byte)rndNumber, DateTime.Now, false);

            database.RunSession(session => BikeUpdateLocation.InsertLocation(session, expected));

            GPSData actual = database.RunSession(session => bike.LatestGPSData(session));

            Assert.AreEqual(expected.Bike, actual.Bike);
            Assert.AreEqual(expected.Accuracy, actual.Accuracy);
            Assert.AreEqual(expected.Location, actual.Location);
            Assert.AreEqual(expected.HasNotMoved, actual.HasNotMoved);

        }

        /// <summary>
        /// Make sure the expected <see cref="GPSData"/> is not already in database, else the test might pass without working.
        /// </summary>
        [TestMethod]
        public void updateLatestLocation()
        {
            var bike = new Bike((uint)rndNumber);
            GPSData expected = new GPSData(bike, new GPSLocation(rndNumber, rndNumber), (byte)rndNumber, DateTime.Now, false);

            database.RunSession(session =>
            {
                BikeUpdateLocation.InsertLocation(session, expected);
                BikeUpdateLocation.InsertLocation(session, expected);
            });

            GPSData actual = database.RunSession(session => bike.LatestGPSData(session));

            Assert.AreEqual(expected.Bike, actual.Bike);
            Assert.AreEqual(expected.Accuracy, actual.Accuracy);
            Assert.AreEqual(expected.Location, actual.Location);
            Assert.AreEqual(true, actual.HasNotMoved);
        }
    }
}
