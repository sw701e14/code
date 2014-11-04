using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LibraryTests
{
    [TestClass]
    public class BikeUpdateLocationTests
    {
        Library.GeneratedDatabaseModel.Database database = new Library.GeneratedDatabaseModel.Database();

        [TestMethod]
        public void NullParameter()
        {
            var allGpsData = database.gps_data;




            Library.BikeUpdateLocation.InsertLocation(null);
        }
    }
}
