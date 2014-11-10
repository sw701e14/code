﻿using Library.GeneratedDatabaseModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace DatabaseImport
{
    /// <summary>
    /// Exposes methods for retrieving <see cref="gps_data"/>s from a Google Directions route.
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
        /// Queries Google Directions with a URL and generates a set of <see cref="gps_data"/>s representing the generated route.
        /// </summary>
        /// <param name="url">The URL from which Google Directions should generate a route.</param>
        /// <param name="startTime">The time associated with the first <see cref="gps_data"/> in the result.</param>
        /// <param name="bikeId">The bike identifier.</param>
        /// <returns>A collection of <see cref="gps_data"/>s representing the generated route.</returns>
        public static IEnumerable<gps_data> GetData(string url, DateTime startTime, int bikeId)
        {
            var parser = new GoogleDirectionsParser(startTime, bikeId);
            return parser.loadPoints(url);
        }

        /// <summary>
        /// Queries Google Directions with a from and a to string and generates a set of <see cref="gps_data"/>s representing the generated route.
        /// </summary>
        /// <param name="from">The location where the route should start (an address).</param>
        /// <param name="to">The location where the route should end (an address).</param>
        /// <param name="startTime">The time associated with the first <see cref="gps_data"/> in the result.</param>
        /// <param name="bikeId">The bike identifier.</param>
        /// <returns>A collection of <see cref="gps_data"/>s representing the generated route.</returns>
        public static IEnumerable<gps_data> GetData(string from, string to, DateTime startTime, int bikeId)
        {
            string apialexander = "AIzaSyDIy0olG2SFd75gMbdshoEc61wZyzlGLOg";
            string apimikael = "AIzaSyBLIB1DsgmDpNPuhUaFKSMO-SEt2gLA9Vk";
            string apistefan = "AIzaSyCRqTjo_VNze5PlFoPtLHzTM_4MfPIZR7w";
            string apibruno = "AIzaSyCsiJFXbak8ywb8p3GkoJ8Bji2DxgmH78g";

            string url = "https://maps.googleapis.com/maps/api/directions/xml?origin={0}&destination={1}&sensor=false&key="+apibruno +"&avoid=highways&mode=bicycling&language=da";

            string enc = System.Web.HttpUtility.UrlEncode(from);

            url = string.Format(url,
                System.Web.HttpUtility.UrlEncode(from),
                System.Web.HttpUtility.UrlEncode(to));

            return GetData(url, startTime, bikeId);
        }

        private IEnumerable<gps_data> loadPoints(string url)
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

        private gps_data parseLocationData(XElement element, bool last)
        {
            var location = last ? element.Element("end_location") : element.Element("start_location");
            double lat = double.Parse(location.Element("lat").Value, System.Globalization.CultureInfo.InvariantCulture);
            double lng = double.Parse(location.Element("lng").Value, System.Globalization.CultureInfo.InvariantCulture);

            var point = new gps_data(nextDate, (decimal)lat, (decimal)lng, null, bikeId);

            int durationSeconds = int.Parse(element.Element("duration").Element("value").Value);
            nextDate = nextDate.AddSeconds(durationSeconds);

            return point;
        }
    }
}
