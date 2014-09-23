using Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Library
{
    public class GDirectionsParser
    {
        private static DateTime tempDatePlusDuration = DateTime.Now;
        private static int bikeId;

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

        public static XDocument FetchGDirectionData(string url)
        {
            Uri requestUrl = new Uri(url);
            Stream responseStream = null;
            WebRequest request = null;
            HttpWebResponse response = null;

            request = System.Net.WebRequest.Create(requestUrl);
            request.Proxy = null;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            //Allows for validation of SSL certificates 
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, ssl) => true;

            response = (System.Net.HttpWebResponse)request.GetResponse();
            responseStream = response.GetResponseStream();

            return XDocument.Load(responseStream);
        }

        private static IEnumerable<GPSPoint> parseGDirectionToGpsData(XDocument xmlDoc)
        {
            if (xmlDoc == null)
                throw new ArgumentNullException("xmlDoc");

            foreach (var step in xmlDoc.Descendants("step"))
                yield return parseLocationData(step, false);

            yield return parseLocationData(xmlDoc.Descendants("step").Last(), true);
        }

        private static GPSPoint parseLocationData(XElement element, bool last)
        {
            var location = last ? element.Element("end_location") : element.Element("start_location");
            double lat = double.Parse(location.Element("lat").Value, System.Globalization.CultureInfo.InvariantCulture);
            double lng = double.Parse(location.Element("lng").Value, System.Globalization.CultureInfo.InvariantCulture);

            var point = new GPSPoint(tempDatePlusDuration, lat, lng, null, bikeId);

            int durationSeconds = int.Parse(element.Element("duration").Element("value").Value);
            tempDatePlusDuration.AddSeconds(durationSeconds);

            return point;
        }
    }
}
