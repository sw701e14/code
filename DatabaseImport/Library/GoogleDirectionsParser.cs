﻿using Library;
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
    public class GoogleDirectionsParser
    {
        private DateTime tempDatePlusDuration = DateTime.Now;
        private int bikeId;

        private GoogleDirectionsParser(DateTime startTime, int bikeId)
        {
            this.tempDatePlusDuration = startTime;
            this.bikeId = bikeId;
        }

        public static IEnumerable<GPSPoint> FetchGDirectionData(string url, DateTime startTime, int bikeId)
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

            var point = new GPSPoint(tempDatePlusDuration, lat, lng, null, bikeId);

            int durationSeconds = int.Parse(element.Element("duration").Element("value").Value);
            tempDatePlusDuration.AddSeconds(durationSeconds);

            return point;
        }
    }
}
