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
    /// <summary>
    /// Exposes methods for retrieving <see cref="GPSPoint"/>s from a Google Directions route.
    /// </summary>
    public class GoogleDirectionsParser
    {
        private DateTime nextDate;
        private int bikeId;

        private GoogleDirectionsParser(DateTime startTime, int bikeId)
        {
            this.nextDate = startTime;
            this.bikeId = bikeId;
        }

        /// <summary>
        /// Queries Google Directions with a URL and generates a set of <see cref="GPSPoint"/>s representing the generated route.
        /// </summary>
        /// <param name="url">The URL from which Google Directions should generate a route.</param>
        /// <param name="startTime">The time associated with the first <see cref="GPSPoint"/> in the result.</param>
        /// <param name="bikeId">The bike identifier.</param>
        /// <returns>A collection of <see cref="GPSPoint"/>s representing the generated route.</returns>
        public static IEnumerable<GPSPoint> GetData(string url, DateTime startTime, int bikeId)
        {
            var parser = new GoogleDirectionsParser(startTime, bikeId);
            return parser.loadPoints(url);
        }

        private IEnumerable<GPSPoint> loadPoints(string url)
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

            XDocument xmlDoc = XDocument.Load(responseStream);

            foreach (var step in xmlDoc.Descendants("step"))
                yield return parseLocationData(step, false);

            yield return parseLocationData(xmlDoc.Descendants("step").Last(), true);
        }

        private GPSPoint parseLocationData(XElement element, bool last)
        {
            var location = last ? element.Element("end_location") : element.Element("start_location");
            double lat = double.Parse(location.Element("lat").Value, System.Globalization.CultureInfo.InvariantCulture);
            double lng = double.Parse(location.Element("lng").Value, System.Globalization.CultureInfo.InvariantCulture);

            var point = new GPSPoint(nextDate, lat, lng, null, bikeId);

            int durationSeconds = int.Parse(element.Element("duration").Element("value").Value);
            nextDate.AddSeconds(durationSeconds);

            return point;
        }
    }
}
