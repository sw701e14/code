using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library;

namespace LibraryTests
{
    [TestClass]
    public class BikeUpdateLocationTests
    {
        Database database = new Database();
        int rndNumber = 50;

        /// <summary>
        /// Make sure the expected <see cref="GPSData"/> is not already in database, else the test will fail.
        /// </summary>
        [TestMethod]
        public void addNewLocation()
        {

            GPSData expected = new GPSData(DateTime.Now, rndNumber, rndNumber, rndNumber, rndNumber);
            expected.hasNotMoved = false;
            
            Library.BikeUpdateLocation.InsertLocation(expected);

            GPSData actual = database.GPSData.Where(x => x.bikeId == rndNumber && x.accuracy == (byte)rndNumber && x.latitude == rndNumber && x.longitude == rndNumber).FirstOrDefault();

            Assert.AreEqual(expected.bikeId, actual.bikeId);
            Assert.AreEqual(expected.accuracy, actual.accuracy);
            Assert.AreEqual(expected.latitude, actual.latitude);
            Assert.AreEqual(expected.longitude, actual.longitude);
            Assert.AreEqual(expected.hasNotMoved, actual.hasNotMoved);

        }

        /// <summary>
        /// Make sure the expected <see cref="GPSData"/> is not already in database, else the test might pass without working.
        /// </summary>
        [TestMethod]
        public void updateLatestLocation()
        {
            GPSData expected = new GPSData(DateTime.Now, rndNumber, rndNumber, rndNumber, rndNumber);
            expected.hasNotMoved = false;

            Library.BikeUpdateLocation.InsertLocation(expected);
            Library.BikeUpdateLocation.InsertLocation(expected);

            GPSData actual = database.GPSData.Where(x => x.bikeId == rndNumber && x.accuracy == (byte)rndNumber && x.latitude == rndNumber && x.longitude == rndNumber).FirstOrDefault();

            Assert.AreEqual(expected.bikeId, actual.bikeId);
            Assert.AreEqual(expected.accuracy, actual.accuracy);
            Assert.AreEqual(expected.latitude, actual.latitude);
            Assert.AreEqual(expected.longitude, actual.longitude);
            Assert.AreEqual(true, actual.hasNotMoved);
        }
    }
}
