using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ConsoleApplication1
{
    public class AddTestDataWithGDirections
    {
        private static DateTime tempDateTime = DateTime.Now;
        private static DateTime tempDatePlusDuration = DateTime.Now;
        private static double tempStartLatitude;
        private static double tempStartLongtitude;
        private static double tempEndLatitude;
        private static double tempEndLongtitude;

        public static void Main(string[] urls)
        {
            string testString = "https://maps.googleapis.com/maps/api/directions/xml?origin=39+Kastetvej,+Aalborg,+Nordjylland,+Danmark&destination=300+Selma+Lagerl%C3%B8fs+Vej,+Aalborg+%C3%98st,+Nordjylland,+Danmark&sensor=false&key=AIzaSyBLIB1DsgmDpNPuhUaFKSMO-SEt2gLA9Vk&avoid=highways&mode=bicycling";
            ParseGDirectionToGpsData(FetchGDirectionData(testString));

            foreach (string url in urls)
            {
                ParseGDirectionToGpsData(FetchGDirectionData(url));
            }
        }

        private static XmlDocument FetchGDirectionData(string url)
        {
            Uri requestUrl = new Uri(@url);
            System.IO.Stream responseStream = null;
            XmlDocument xmlDoc = new XmlDocument();
            System.Net.WebRequest request = null;
            System.Net.HttpWebResponse response = null;
            System.IO.StreamReader responseReader = null;
            string responseString = null;

            try
            {
                request = System.Net.WebRequest.Create(requestUrl);
                request.Proxy = null;
                request.Credentials = System.Net.CredentialCache.DefaultCredentials;
            }
            catch (ArgumentNullException e)
            {
                throw;
            }
            catch (NotSupportedException e)
            {
                throw;
            }
            catch (System.Security.SecurityException e)
            {
                throw;
            }

            //Allows for validation of SSL certificates 
            System.Net.ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);

            try
            {
                response = (System.Net.HttpWebResponse)request.GetResponse();
            }
            catch (NotImplementedException e)
            {
                throw;
            }

            try
            {
                responseStream = response.GetResponseStream();
            }
            catch (System.Net.ProtocolViolationException e)
            {
                throw;
            }
            catch (ObjectDisposedException e)
            {
                throw;
            }

            try
            {
                responseReader = new System.IO.StreamReader(responseStream);
            }
            catch (ArgumentNullException e)
            {
                throw;
            }
            catch (ArgumentException e)
            {
                throw;
            }

            try
            {
                responseString = responseReader.ReadToEnd();
            }
            catch (OutOfMemoryException e)
            {
                throw;
            }
            catch (System.IO.IOException e)
            {
                throw;
            }

            try
            {
                xmlDoc.LoadXml(responseString);
            }
            catch (XmlException e)
            {
                throw;
            }

            return xmlDoc;
        }

        //for testing purpose only, accept any dodgy certificate... 
        private static bool ValidateServerCertificate(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static void ParseGDirectionToGpsData(XmlDocument xmlDoc)
        {
            if (xmlDoc == null)
            {
                throw new ArgumentNullException(@"inputStream is null");
            }

            XmlNodeList xmlNodeList = xmlDoc.GetElementsByTagName("step");

            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                parseLocationData(xmlNodeList[i], false);
                new GPSPoint(tempDateTime, tempStartLatitude, tempStartLongtitude, -1, -1);
                //AddDataToDatabase()

                /*
                Console.WriteLine("GPSPoint {0}", i);
                Console.WriteLine("Start Lat: {0}", tempStartLatitude);
                Console.WriteLine("Start Lng: {0}", tempStartLongtitude);
                Console.WriteLine("Time: {0}\n", tempDateTime);
                */

                if (i == (xmlNodeList.Count - 1))
                {
                    parseLocationData(xmlNodeList[i], true);
                    new GPSPoint(tempDatePlusDuration, tempEndLatitude, tempEndLongtitude, -1, -1);
                    //AddDataToDatabase()

                    /*
                    Console.WriteLine("GPSPoint {0}", i+1);
                    Console.WriteLine("End Lat: {0}", tempEndLatitude);
                    Console.WriteLine("End Lng: {0}", tempEndLongtitude);
                    Console.WriteLine("Time: {0}\n", tempDatePlusDuration);
                    */
                }

                //Console.ReadKey();
            }
        }

        private static void parseLocationData(XmlNode xmlNode, bool last)
        {
            //Timestamp
            tempDateTime = tempDatePlusDuration;
            //start_location lat
            tempStartLatitude = Double.Parse(xmlNode.FirstChild.NextSibling.FirstChild.InnerText,  System.Globalization.CultureInfo.InvariantCulture);
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
