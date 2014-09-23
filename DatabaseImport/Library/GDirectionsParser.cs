using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Library
{
    public class GDirectionsParser
    {
        private static DateTime tempDateTime = DateTime.Now;
        private static DateTime tempDatePlusDuration = DateTime.Now;
        private static double tempStartLatitude;
        private static double tempStartLongtitude;
        private static double tempEndLatitude;
        private static double tempEndLongtitude;

        /// <summary>
        /// Main that adds all gps point from the the specified urls (GDirections).
        /// </summary>
        /// <param name="urls">The urls.</param>
        public static void Main(string[] urls)
        {
            /*//For testing purposes.
            string testString = "https://maps.googleapis.com/maps/api/directions/xml?origin=39+Kastetvej,+Aalborg,+Nordjylland,+Danmark&destination=300+Selma+Lagerl%C3%B8fs+Vej,+Aalborg+%C3%98st,+Nordjylland,+Danmark&sensor=false&key=AIzaSyBLIB1DsgmDpNPuhUaFKSMO-SEt2gLA9Vk&avoid=highways&mode=bicycling&language=da";
            try
            {
                parseGDirectionToGpsData(FetchGDirectionData(testString));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }*/

            foreach (string url in urls)
            {
                try
                {
                    parseGDirectionToGpsData(FetchGDirectionData(url));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                }
            }
        }

        public static XmlDocument FetchGDirectionData(string url)
        {
            XmlDocument xmlDoc = null;
            Uri requestUrl = new Uri(url);
            Stream responseStream = null;
            WebRequest request = null;
            HttpWebResponse response = null;
            StreamReader responseReader = null;
            string responseString = null;

            try
            {
                request = System.Net.WebRequest.Create(requestUrl);
                request.Proxy = null;
                request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            catch (ArgumentNullException e)
            {
                return null;
            }
            catch (NotSupportedException e)
            {
                return null;
            }
            catch (System.Security.SecurityException e)
            {
                return null;
            }

            //Allows for validation of SSL certificates 
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, ssl) => true;

            try
            {
                response = (System.Net.HttpWebResponse)request.GetResponse();
            }
            catch (NotImplementedException e)
            {
                return null;
            }

            try
            {
                responseStream = response.GetResponseStream();
            }
            catch (System.Net.ProtocolViolationException e)
            {
                return null;
            }
            catch (ObjectDisposedException e)
            {
                return null;
            }

            try
            {
                responseReader = new System.IO.StreamReader(responseStream);
            }
            catch (ArgumentNullException e)
            {
                return null;
            }
            catch (ArgumentException e)
            {
                return null;
            }

            try
            {
                responseString = responseReader.ReadToEnd();
            }
            catch (OutOfMemoryException e)
            {
                return null;
            }
            catch (System.IO.IOException e)
            {
                return null;
            }

            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(responseString);
            }
            catch (XmlException e)
            {
                return null;
            }

            return xmlDoc;
        }

        private static void parseGDirectionToGpsData(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
            {
                throw new ArgumentNullException(@"xmlDoc is null");
            }

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName("step");

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                parseLocationData(xmlNodeList[i], false);
                new GPSPoint(tempDateTime, tempStartLatitude, tempStartLongtitude, -1, -1);
                //AddDataToDatabase()

                //Adds the last point in the route - given its different position in the xml document
                if (i == (xmlNodeList.Count - 1))
                {
                    parseLocationData(xmlNodeList[i], true);
                    new GPSPoint(tempDatePlusDuration, tempEndLatitude, tempEndLongtitude, -1, -1);
                    //AddDataToDatabase()
                }
            }
        }

        //
        private static void parseLocationData(XmlNode xmlNode, bool last)
        {
            //Timestamp
            tempDateTime = tempDatePlusDuration;
            //start_location lat
            tempStartLatitude = Double.Parse(xmlNode.FirstChild.NextSibling.FirstChild.InnerText, System.Globalization.CultureInfo.InvariantCulture);
            //start_location lng
            tempStartLongtitude = Double.Parse(xmlNode.FirstChild.NextSibling.FirstChild.NextSibling.InnerText, System.Globalization.CultureInfo.InvariantCulture);
            //duration
            double tempDurationInSeconds = Double.Parse(xmlNode.FirstChild.NextSibling.NextSibling.NextSibling.NextSibling.FirstChild.InnerText, System.Globalization.CultureInfo.InvariantCulture);
            TimeSpan duration = TimeSpan.FromSeconds(tempDurationInSeconds);
            tempDatePlusDuration = tempDatePlusDuration.Add(duration);

            if (last)
            {
                //end_location lat
                tempEndLatitude = Double.Parse(xmlNode.FirstChild.NextSibling.NextSibling.FirstChild.InnerText, System.Globalization.CultureInfo.InvariantCulture);
                //end_location lng
                tempEndLongtitude = Double.Parse(xmlNode.FirstChild.NextSibling.NextSibling.FirstChild.NextSibling.InnerText, System.Globalization.CultureInfo.InvariantCulture);
            }
        }
    }
}
