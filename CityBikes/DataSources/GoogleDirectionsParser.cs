using DataLoading.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Linq;

namespace DataLoading.LocationSource
{
    /// <summary>
    /// Exposes methods for retrieving <see cref="GPSData"/>s from a Google Directions route.
    /// </summary>
    public class GoogleDirectionsParser
    {
        private DateTime nextDate;
        private uint bike;

        private GoogleDirectionsParser(DateTime startTime, uint bike)
        {
            this.nextDate = startTime;
            this.bike = bike;
        }

        /// <summary>
        /// Queries Google Directions with a from and a to string and generates a set of <see cref="GPSData"/>s representing the generated route.
        /// </summary>
        /// <param name="from">The location where the route should start (an address).</param>
        /// <param name="to">The location where the route should end (an address).</param>
        /// <param name="startTime">The time associated with the first <see cref="GPSData"/> in the result.</param>
        /// <param name="bikeId">The bike identifier.</param>
        /// <returns>A collection of <see cref="GPSData"/>s representing the generated route.</returns>
        public static IEnumerable<GPSInput> GetData(string from, string to, DateTime startTime, uint bike)
        {
            string file = getFileName(from, to);
            XDocument xml;

            var parser = new GoogleDirectionsParser(startTime, bike);

            if (File.Exists(file))
            {
                xml = XDocument.Load(file);
            }
            else
            {
                string apialexander = "AIzaSyDIy0olG2SFd75gMbdshoEc61wZyzlGLOg";
                string apimikael = "AIzaSyBLIB1DsgmDpNPuhUaFKSMO-SEt2gLA9Vk";
                string apistefan = "AIzaSyCRqTjo_VNze5PlFoPtLHzTM_4MfPIZR7w";
                string apibruno = "AIzaSyCsiJFXbak8ywb8p3GkoJ8Bji2DxgmH78g";

                string API = apialexander;

                string url = "https://maps.googleapis.com/maps/api/directions/xml?origin={0}&destination={1}&sensor=false&key=" + API + "&avoid=highways&mode=bicycling&language=da";

                string enc = System.Web.HttpUtility.UrlEncode(from);

                url = string.Format(url,
                    System.Web.HttpUtility.UrlEncode(from),
                    System.Web.HttpUtility.UrlEncode(to));

                xml = parser.downloadXML(url);
                xml.Save(file);
            }
            return parser.loadPoints(xml);
        }

        private static MD5 md5 = MD5.Create();
        private static string getHash(string text)
        {
            var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(text));
            return string.Concat(hash.Select(b => b.ToString("X2")).ToArray());
        }
        private static string getFileName(string from, string to)
        {
            DirectoryInfo dir;
            if (!Directory.Exists("google"))
                dir = Directory.CreateDirectory("google");

            return "google/" + getHash(from) + "_" + getHash(to);
        }

        private XDocument downloadXML(string url)
        {
        retry:

            Uri requestUrl = new Uri(url);
            WebRequest request = null;
            HttpWebResponse response = null;
            XDocument xmlDoc = null;

            request = System.Net.WebRequest.Create(requestUrl);
            request.Proxy = null;
            request.Credentials = System.Net.CredentialCache.DefaultCredentials;

            //Allows for validation of SSL certificates 
            System.Net.ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, ssl) => true;

            try
            {
                response = (System.Net.HttpWebResponse)request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    xmlDoc = XDocument.Load(responseStream);
                }
                var status = xmlDoc.Element("DirectionsResponse").Element("status").Value;

                if (status != "OK")
                {
                    response.Dispose();
                    System.Threading.Thread.Sleep(500);
                    goto retry;
                }
            }
            catch (WebException e)
            {
                goto retry;
            }
            finally
            {
                if (response != null)
                    response.Dispose();
            }

            return xmlDoc;
        }

        private IEnumerable<GPSInput> loadPoints(XDocument xmlDoc)
        {
            foreach (var step in xmlDoc.Descendants("step"))
                yield return parseLocationData(step, false);

            yield return parseLocationData(xmlDoc.Descendants("step").Last(), true);
        }

        private GPSInput parseLocationData(XElement element, bool last)
        {
            var location = last ? element.Element("end_location") : element.Element("start_location");
            double lat = double.Parse(location.Element("lat").Value, System.Globalization.CultureInfo.InvariantCulture);
            double lng = double.Parse(location.Element("lng").Value, System.Globalization.CultureInfo.InvariantCulture);

            var point = new GPSInput(bike, (decimal)lat, (decimal)lng, null, nextDate);

            int durationSeconds = int.Parse(element.Element("duration").Element("value").Value);
            nextDate = nextDate.AddSeconds(durationSeconds);

            return point;
        }
    }
}
