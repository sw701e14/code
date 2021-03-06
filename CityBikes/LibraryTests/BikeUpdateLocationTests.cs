﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shared.DAL;
using Shared.DTO;

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

            database.RunSession(session => GPSData.InsertInDatabase(session, expected));

            GPSData? actual = database.RunSession(session => Bike.GetLatestData(session, bike));

            Assert.IsNotNull(actual);
            if (actual.HasValue)
            {
                Assert.AreEqual(expected.Bike, actual.Value.Bike);
                Assert.AreEqual(expected.Accuracy, actual.Value.Accuracy);
                Assert.AreEqual(expected.Location, actual.Value.Location);
                Assert.AreEqual(expected.HasNotMoved, actual.Value.HasNotMoved);
            }
        }

        /// <summary>
        /// Make sure the expected <see cref="GPSData"/> is not already in database, else the test might pass without working.
        /// </summary>
        [TestMethod]
        public void updateLatestLocation()
        {
            var bike = new Bike((uint)rndNumber);
            GPSData expected = new GPSData(bike, new GPSLocation(rndNumber, rndNumber), (byte)rndNumber, DateTime.Now, true);

            database.RunSession(session =>
            {
                GPSData.InsertInDatabase(session, expected);
                GPSData.InsertInDatabase(session, expected);
            });

            GPSData? actual = database.RunSession(session => Bike.GetLatestData(session, bike));
            
            Assert.IsNotNull(actual);
            if (actual.HasValue)
            {
                Assert.AreEqual(expected.Bike, actual.Value.Bike);
                Assert.AreEqual(expected.Accuracy, actual.Value.Accuracy);
                Assert.AreEqual(expected.Location, actual.Value.Location);
                Assert.AreEqual(expected.HasNotMoved, actual.Value.HasNotMoved);
            }
        }
    }
}
