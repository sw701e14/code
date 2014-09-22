using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace DatabaseImportTest
{
    [TestClass]
    public class GDirectionsParserTests
    {
        string testGDirectionUrl;
        GPSPoint gpsPoint1;

        [TestInitialize]
        public void Setup()
        {
            testGDirectionUrl = "https://maps.googleapis.com/maps/api/directions/xml?origin=39+Kastetvej,+Aalborg,+Nordjylland,+Danmark&destination=300+Selma+Lagerl%C3%B8fs+Vej,+Aalborg+%C3%98st,+Nordjylland,+Danmark&sensor=false&key=AIzaSyBLIB1DsgmDpNPuhUaFKSMO-SEt2gLA9Vk&avoid=highways&mode=bicycling&language=da";
        }


        [TestMethod]
        public void GetXMLDocumentWithCorrectUrl()
        {
            XmlDocument actualXmlDoc = Library.GDirectionsParser.FetchGDirectionData(testGDirectionUrl);
            string actualString = actualXmlDoc.InnerXml;

            XmlDocument expectedXmlDoc = new XmlDocument();
            expectedXmlDoc.Load(@"..\..\..\..\TestData\" + "GDirectionsTestXml.xml");
            string expectedString = expectedXmlDoc.InnerXml;

            Assert.AreEqual(expectedString, actualString);
        }

        [TestMethod]
        public void GetXMLDocumentWithWrongUrl()
        {
            XmlDocument actual = Library.GDirectionsParser.FetchGDirectionData("");
            XmlDocument expected = null;

            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void GetXMLDocumentWithNullUrl()
        {
            XmlDocument actual = Library.GDirectionsParser.FetchGDirectionData(null);
            XmlDocument expected = null;

            Assert.AreEqual(expected, actual);
        }
    }
}
