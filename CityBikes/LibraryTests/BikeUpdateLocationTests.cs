using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests
{
    [TestClass]
    public class BikeUpdateLocationTests
    {
        Library.GeneratedDatabaseModel.Database database = new Library.GeneratedDatabaseModel.Database();
        int rndNumber = 50;

        [TestMethod]
        public void NullParameter()
        {
            bool expected = true;
            bool actual = false;
            try
            {
                Library.BikeUpdateLocation.InsertLocation(database, null);
            }
            catch (ArgumentNullException)
            {
                actual = true;
            }

            Assert.AreEqual(expected, actual);
        }



        /// <summary>
        /// Make sure the expected gps_data is not already in database, else the test will fail.
        /// </summary>
        [TestMethod]
        public void addNewLocation()
        {

            Library.GeneratedDatabaseModel.gps_data expected = new Library.GeneratedDatabaseModel.gps_data(DateTime.Now, rndNumber, rndNumber, rndNumber, rndNumber);
            expected.hasNotMoved = false;
            
            Library.BikeUpdateLocation.InsertLocation(database, expected);

            Library.GeneratedDatabaseModel.gps_data actual = database.gps_data.Where(x => x.bikeId == rndNumber && x.accuracy == (byte)rndNumber && x.latitude == rndNumber && x.longitude == rndNumber).FirstOrDefault();

            Assert.AreEqual(expected.bikeId, actual.bikeId);
            Assert.AreEqual(expected.accuracy, actual.accuracy);
            Assert.AreEqual(expected.latitude, actual.latitude);
            Assert.AreEqual(expected.longitude, actual.longitude);
            Assert.AreEqual(expected.hasNotMoved, actual.hasNotMoved);

        }

        /// <summary>
        /// Make sure the expected gps_data is not already in database, else the test might pass without working.
        /// </summary>
        [TestMethod]
        public void updateLatestLocation()
        {
            Library.GeneratedDatabaseModel.gps_data expected = new Library.GeneratedDatabaseModel.gps_data(DateTime.Now, rndNumber, rndNumber, rndNumber, rndNumber);
            expected.hasNotMoved = false;

            Library.BikeUpdateLocation.InsertLocation(database, expected);
            Library.BikeUpdateLocation.InsertLocation(database, expected);

            Library.GeneratedDatabaseModel.gps_data actual = database.gps_data.Where(x => x.bikeId == rndNumber && x.accuracy == (byte)rndNumber && x.latitude == rndNumber && x.longitude == rndNumber).FirstOrDefault();

            Assert.AreEqual(expected.bikeId, actual.bikeId);
            Assert.AreEqual(expected.accuracy, actual.accuracy);
            Assert.AreEqual(expected.latitude, actual.latitude);
            Assert.AreEqual(expected.longitude, actual.longitude);
            Assert.AreEqual(true, actual.hasNotMoved);
        }
    }
}
