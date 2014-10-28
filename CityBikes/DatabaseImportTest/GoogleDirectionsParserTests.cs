using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseImport;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Library.GeneratedDatabaseModel;


namespace DatabaseImportTest
{
    [TestClass]
    public class GoogleDirectionsParserTests
    {
        string testGDirectionUrl;
        gps_data gpsPoint1;

        [TestInitialize]
        public void Setup()
        {
            testGDirectionUrl = "https://maps.googleapis.com/maps/api/directions/xml?origin=39+Kastetvej,+Aalborg,+Nordjylland,+Danmark&destination=300+Selma+Lagerl%C3%B8fs+Vej,+Aalborg+%C3%98st,+Nordjylland,+Danmark&sensor=false&key=AIzaSyBLIB1DsgmDpNPuhUaFKSMO-SEt2gLA9Vk&avoid=highways&mode=bicycling&language=da";
        }
    }
}
