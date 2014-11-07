using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;
using Google.Maps;
using System.IO;
using HtmlAgilityPack;


namespace Library
{
    /// <summary>
    /// 
    /// </summary>
    public static class GPSPointMapPlotter
    {
        private const string API_KEY = "AIzaSyDY0kkJiTPVd2U7aTOAwhc9ySH6oHxOIYM";
        private const string CENTER_LATITUDE = "57.0338295";
        private const string CENTER_LONGITUDE = "9.9277601";
        private const string ZOOM = "12";
        private const string MAP_WIDTH = "600";
        private const string MAP_HEIGHT = "600";

        /// <summary>
        /// Plots ALL gps_data from the database to a Google Map and connects them with lines.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <returns></returns>
        public static HtmlDocument PlotAllGPSPointsToMap(this Database context)
        {
            if (context == null)
            {
                throw new ArgumentNullException();
            }
            else if (String.IsNullOrEmpty(API_KEY) || String.IsNullOrEmpty(CENTER_LATITUDE) || String.IsNullOrEmpty(CENTER_LONGITUDE) ||
                     String.IsNullOrEmpty(ZOOM) || String.IsNullOrEmpty(MAP_WIDTH) || String.IsNullOrEmpty(MAP_HEIGHT))
            {
                throw new ArgumentException("Arguments must not be null or empty.", "Please specify non empty arguments.");
            }

            IQueryable<gps_data> locationList = from locations in context.gps_data
                                                select locations;

            return PlotSelectedGPSPointsToMap(context, locationList);
        }

        /// <summary>
        /// Plots the selected/given gps_points to a Google Map and connects them with lines.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="selectedGPSPoints">The selected GPS points.</param>
        /// <returns></returns>
        public static HtmlDocument PlotSelectedGPSPointsToMap(this Database context, IQueryable<gps_data> selectedGPSPoints)
        {
            if (context == null || selectedGPSPoints == null)
            {
                throw new ArgumentNullException();
            }
            else if (String.IsNullOrEmpty(API_KEY) || String.IsNullOrEmpty(CENTER_LATITUDE) || String.IsNullOrEmpty(CENTER_LONGITUDE) || 
                     String.IsNullOrEmpty(ZOOM) || String.IsNullOrEmpty(MAP_WIDTH) || String.IsNullOrEmpty(MAP_HEIGHT))
            {
                throw new ArgumentException("Arguments must not be null or empty.", "Please specify non empty arguments.");
            }

            HtmlDocument htmlDocument = new HtmlDocument();   

            htmlDocument.LoadHtml(writeHTMLStart() +
                                   writeHTMLSource(API_KEY) +
                                   writeHTMLMapCenter(CENTER_LATITUDE, CENTER_LONGITUDE) +
                                   writeHTMLMapSetting(ZOOM) +
                                   writeHTMLPointsAndLines(selectedGPSPoints) +
                                   writeHTMLAddMarkerMethod() +
                                   writeHTMLDisplayMap(MAP_WIDTH, MAP_HEIGHT));

            return htmlDocument;
        }

        /// <summary>
        /// Creates HTML syntax and begins header.
        /// </summary>
        /// <returns></returns>
        private static string writeHTMLStart()
        {
            string result = "";

            result = result + "<!DOCTYPE html>" + System.Environment.NewLine;
            result = result + "<html>" + System.Environment.NewLine;
            result = result + "<head>" + System.Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Creates the source to use the Google Maps API thorugh the apiKey.
        /// </summary>
        /// <param name="apiKey">The API key.</param>
        /// <returns></returns>
        private static string writeHTMLSource(string apiKey)
        {
            string result = "";

            result = result + "<script" + System.Environment.NewLine;
            result = result + "src=\"http://maps.googleapis.com/maps/api/js?key=" + apiKey + "&sensor=false\">" + System.Environment.NewLine;
            result = result + "</script>" + System.Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Determines the coordinates for the center of the map.
        /// </summary>
        /// <param name="centerLatitude">The center latitude.</param>
        /// <param name="centerLongtitude">The center longtitude.</param>
        /// <returns></returns>
        private static string writeHTMLMapCenter(string centerLatitude, string centerLongtitude)
        {
            string result = "";

            result = result + "<script>" + System.Environment.NewLine;
            result = result + "var map;" + System.Environment.NewLine;
            result = result + "var myCenter=new google.maps.LatLng(" + centerLatitude + "," + centerLongtitude + ");" + System.Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Determines the settings for the Google Map.
        /// </summary>
        /// <param name="zoom">The zoom.</param>
        /// <returns></returns>
        private static string writeHTMLMapSetting(string zoom)
        {
            string result = "";

            result = result + "function initialize(){" + System.Environment.NewLine;
            result = result + "var mapProp = {" + System.Environment.NewLine;
            result = result + "center:myCenter," + System.Environment.NewLine;
            result = result + "zoom:" + zoom + "," + System.Environment.NewLine;
            result = result + "mapTypeId:google.maps.MapTypeId.ROADMAP};" + System.Environment.NewLine;
            result = result + "map = new google.maps.Map(document.getElementById(\"googleMap\"),mapProp);" + System.Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Draws all points and lines.
        /// </summary>
        /// <param name="locationList">The location list.</param>
        /// <returns></returns>
        private static string writeHTMLPointsAndLines(IQueryable<gps_data> locationList)
        {
            string result = "";
            gps_data previousData = null;
            string lineString = "";
            gps_data lastInList = locationList.AsEnumerable().Last();

            foreach (gps_data bikeLocation in locationList)
            {
                if (!(previousData == null))
                {
                    if (previousData.bikeId == bikeLocation.bikeId)
                    {
                        //Writes line variable (eg. var myLine1) and add connection points for the line.
                        if (!(lineString.StartsWith("var myLine" + bikeLocation.bikeId + "=[")))
                        {
                            lineString = "var myLine" + bikeLocation.bikeId + "=[";
                        }
                        lineString = lineString + "new google.maps.LatLng(" + bikeLocation.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),";
                    }
                    else
                    {
                        //Writes method and settings for creating lines for this bikeID.
                        result = result + writeHTMLLine(lineString, bikeLocation.bikeId - 1, bikeLocation.queried);
                        lineString = "var myLine" + bikeLocation.bikeId + "=[";
                        lineString = lineString + "new google.maps.LatLng(" + bikeLocation.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),";
                    }
                }
                else
                {
                    //Writes line variable (eg. var myLine1) and add connection points for the first line.
                    if (!(lineString.StartsWith("var myLine" + bikeLocation.bikeId + "=[")))
                    {
                        lineString = "var myLine" + bikeLocation.bikeId + "=[";
                    }
                    lineString = lineString + "new google.maps.LatLng(" + bikeLocation.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),";
                }

                //Writes all markers.
                result = result + "addMarker(new google.maps.LatLng(" + bikeLocation.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),'" + bikeLocation.bikeId + "','" + bikeLocation.queried.ToString() + "');" + System.Environment.NewLine;

                if (bikeLocation == lastInList)
                {
                    //Writes method and settings for creating lines for the last bikeID.
                    result = result + writeHTMLLine(lineString, bikeLocation.bikeId, bikeLocation.queried);
                }

                previousData = bikeLocation;
            }

            result = result + "}" + System.Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Creates method determining the settings and customization for a bikeIDs line.
        /// </summary>
        /// <param name="lineDataAsString">The line data as string.</param>
        /// <param name="bikeID">The bike identifier.</param>
        /// <param name="queried">The queried.</param>
        /// <returns></returns>
        private static string writeHTMLLine(string lineDataAsString, int bikeID, DateTime queried)
        {
            string result = "";

            result = result + lineDataAsString + "]" + System.Environment.NewLine;

            result = result + "var completeLine" + bikeID + "=new google.maps.Polyline({" + System.Environment.NewLine;
            result = result + "path:myLine" + bikeID + "," + System.Environment.NewLine;
            result = result + "strokeColor:\"#" + calculateColor(bikeID, queried) + "\"," + System.Environment.NewLine;
            result = result + "strokeOpacity:0.8," + System.Environment.NewLine;
            result = result + "strokeWeight:2});" + System.Environment.NewLine;
            result = result + "completeLine" + bikeID + ".setMap(map);" + System.Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Calculates the color of a line.
        /// </summary>
        /// <param name="bikeID">The bike identifier.</param>
        /// <param name="queriedInSeconds">The queried in seconds.</param>
        /// <returns></returns>
        private static string calculateColor(int bikeID, DateTime queriedInSeconds)
        {
            double maxHexValue = 16777215;
            int colorInDouble = (int) Math.Abs(((queriedInSeconds.Year + 1) *
                                                (queriedInSeconds.Month + 1) *
                                                (queriedInSeconds.DayOfYear + 1) * 
                                                (queriedInSeconds.Day + 1) *
                                                (queriedInSeconds.Hour + 1) *
                                                (queriedInSeconds.Minute + 1) *
                                                (queriedInSeconds.Second + 1)) % maxHexValue);

            string hexValue = colorInDouble.ToString("X");

            return hexValue;
        }

        /// <summary>
        /// Generates the addMarker method.
        /// </summary>
        /// <returns></returns>
        private static string writeHTMLAddMarkerMethod()
        {
            string result = "";

            result = result + "function addMarker(location,bikeID, date) {" + System.Environment.NewLine;
            result = result + "var marker = new google.maps.Marker({" + System.Environment.NewLine;
            result = result + "position: location," + System.Environment.NewLine;
            result = result + "map: map," + System.Environment.NewLine;
            result = result + "icon: 'http://maps.google.com/mapfiles/ms/icons/blue-dot.png'});" + System.Environment.NewLine;
            result = result + "var infowindow = new google.maps.InfoWindow({" + System.Environment.NewLine;
            result = result + "content: 'BikeID: ' + bikeID + '<br>Date: ' + date+ '<br>Latitude: ' + location.lat() + '<br>Longitude: ' + location.lng()" + "});" + System.Environment.NewLine;
            result = result + "google.maps.event.addListener(marker, 'click', function() {" + System.Environment.NewLine;
            result = result + "infowindow.open(map,marker);" + "});}" + System.Environment.NewLine;

            return result;
        }

        /// <summary>
        /// Writes the body which displays the Google Map.
        /// </summary>
        /// <param name="sizeWidth">Width of the size.</param>
        /// <param name="sizeHeight">Height of the size.</param>
        /// <returns></returns>
        private static string writeHTMLDisplayMap(string sizeWidth, string sizeHeight)
        {
            string result = "";

            result = result + "google.maps.event.addDomListener(window, 'load', initialize);" + System.Environment.NewLine;
            result = result + "</script>" + System.Environment.NewLine;
            result = result + "</head>" + System.Environment.NewLine;
            result = result + "<body>" + System.Environment.NewLine;
            result = result + "<div id=\"googleMap\" style=\"width:" + sizeWidth + "px;height:" + sizeHeight + "px;\"></div>" + System.Environment.NewLine;
            result = result + "</body>" + System.Environment.NewLine;
            result = result + "</html>";

            return result;
        }
    }
}
