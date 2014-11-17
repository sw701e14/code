using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.IO;
using HtmlAgilityPack;


namespace DatabaseImport
{
    /// <summary>
    /// Provides methods that creates a Google Map with plotted in GPS locations.
    /// Maps are saved to working directory (default: /run/Debug/)
    /// </summary>
    public static class GPSPointMapPlotter
    {
        //Setup for Google Maps API
        private const string API_KEY = "AIzaSyDY0kkJiTPVd2U7aTOAwhc9ySH6oHxOIYM";
        private const string CENTER_LATITUDE = "57.0338295";
        private const string CENTER_LONGITUDE = "9.9277601";
        private const string ZOOM = "12";
        private const string MAP_WIDTH = "600";
        private const string MAP_HEIGHT = "600";

        /// <summary>
        /// Plots ALL <see cref="GPSData"/> from database to a Google Map and connects same-bike-id-points with lines.
        /// Saves the map as a HTML-file in working directory.
        /// </summary>
        /// <param name="session">The <see cref="Database.DatabaseSession"/> used to load gps data.</param>
        public static void SaveMapAsHtml(this Database.DatabaseSession session)
        {
            if (session == null)
                throw new ArgumentNullException();

            GPSData[] data = session.ExecuteRead("SElECT * FROM citybikes_test.gps_data").Select(r => r.GetGPSData()).ToArray();

            SaveMapAsHtml(data);
        }

        /// <summary>
        /// Plots the given <see cref="GPSData"/> to a Google Map and connects same-bike-id-points with lines.
        /// Saves the map as a HTML-file in working directory.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="selectedGPSPoints">The selected GPS data.</param>
        public static void SaveMapAsHtml(IEnumerable<GPSData> selectedGPSPoints)
        {
            if (selectedGPSPoints == null)
                throw new ArgumentNullException();

            HtmlDocument htmlDocument = new HtmlDocument();

            htmlDocument.LoadHtml(writeHTMLStart() +
                                   writeHTMLSource(API_KEY) +
                                   writeHTMLMapCenter(CENTER_LATITUDE, CENTER_LONGITUDE) +
                                   writeHTMLMapSetting(ZOOM) +
                                   writeHTMLPointsAndLines(selectedGPSPoints) +
                                   writeHTMLAddMarkerMethod() +
                                   writeHTMLDisplayMap(MAP_WIDTH, MAP_HEIGHT));

            htmlDocument.Save(Path.GetFullPath(".") + "\\map.html");
        }

        private static string writeHTMLStart()
        {
            string result = "";

            result = result + "<!DOCTYPE html>" + System.Environment.NewLine;
            result = result + "<html>" + System.Environment.NewLine;
            result = result + "<head>" + System.Environment.NewLine;

            return result;
        }

        private static string writeHTMLSource(string apiKey)
        {
            string result = "";

            result = result + "<script" + System.Environment.NewLine;
            result = result + "src=\"http://maps.googleapis.com/maps/api/js?key=" + apiKey + "&sensor=false\">" + System.Environment.NewLine;
            result = result + "</script>" + System.Environment.NewLine;

            return result;
        }

        private static string writeHTMLMapCenter(string centerLatitude, string centerLongtitude)
        {
            string result = "";

            result = result + "<script>" + System.Environment.NewLine;
            result = result + "var map;" + System.Environment.NewLine;
            result = result + "var myCenter=new google.maps.LatLng(" + centerLatitude + "," + centerLongtitude + ");" + System.Environment.NewLine;

            return result;
        }

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

        private static string writeHTMLPointsAndLines(IEnumerable<GPSData> locationList)
        {
            string result = "";
            GPSData? previousData = null;
            string lineString = "";
            GPSData lastInList = locationList.AsEnumerable().Last();

            foreach (GPSData bikeLocation in locationList)
            {
                if (previousData.HasValue)
                {
                    if (previousData.Value.Bike == bikeLocation.Bike)
                    {
                        //Writes line variable (eg. var myLine1) and add connection points for the line.
                        if (!(lineString.StartsWith("var myLine" + bikeLocation.Bike.Id + "=[")))
                        {
                            lineString = "var myLine" + bikeLocation.Bike.Id + "=[";
                        }
                        lineString = lineString + "new google.maps.LatLng(" + bikeLocation.Location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.Location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),";
                    }
                    else
                    {
                        //Writes method and settings for creating lines for this bikeID.
                        result = result + writeHTMLLine(lineString, bikeLocation.Bike.Id - 1, bikeLocation.QueryTime);
                        lineString = "var myLine" + bikeLocation.Bike.Id + "=[";
                        lineString = lineString + "new google.maps.LatLng(" + bikeLocation.Location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.Location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),";
                    }
                }
                else
                {
                    //Writes line variable (eg. var myLine1) and add connection points for the first line.
                    if (!(lineString.StartsWith("var myLine" + bikeLocation.Bike.Id + "=[")))
                    {
                        lineString = "var myLine" + bikeLocation.Bike.Id + "=[";
                    }
                    lineString = lineString + "new google.maps.LatLng(" + bikeLocation.Location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.Location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),";
                }

                //Writes all markers.
                result = result + "addMarker(new google.maps.LatLng(" + bikeLocation.Location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.Location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),'" + bikeLocation.Bike.Id + "','" + bikeLocation.QueryTime.ToString() + "');" + System.Environment.NewLine;

                if (bikeLocation.Equals(lastInList))
                {
                    //Writes method and settings for creating lines for the last bikeID.
                    result = result + writeHTMLLine(lineString, bikeLocation.Bike.Id, bikeLocation.QueryTime);
                }

                previousData = bikeLocation;
            }

            result = result + "}" + System.Environment.NewLine;

            return result;
        }

        private static string writeHTMLLine(string lineDataAsString, long bikeID, DateTime queried)
        {
            string result = "";

            result = result + lineDataAsString + "]" + System.Environment.NewLine;

            result = result + "var completeLine" + bikeID + "=new google.maps.Polyline({" + System.Environment.NewLine;
            result = result + "path:myLine" + bikeID + "," + System.Environment.NewLine;
            result = result + "strokeColor:" + calculateColor(bikeID, queried) + "," + System.Environment.NewLine;
            result = result + "strokeOpacity:0.8," + System.Environment.NewLine;
            result = result + "strokeWeight:2});" + System.Environment.NewLine;
            result = result + "completeLine" + bikeID + ".setMap(map);" + System.Environment.NewLine;

            return result;
        }

        private static string calculateColor(long bikeID, DateTime queriedInSeconds)
        {

            decimal randomDecimal = ((queriedInSeconds.Year + 1) *
                                    (queriedInSeconds.Month + 1) *
                                    (queriedInSeconds.DayOfYear + 1) *
                                    (queriedInSeconds.Day + 1) *
                                    (queriedInSeconds.Hour + 1) *
                                    (queriedInSeconds.Minute + 1) *
                                    (queriedInSeconds.Second + 1));

            double randomDouble = (double)randomDecimal;

            var temp = Math.Sqrt(randomDouble) % 8;

            double randomNumber = Math.Floor((double)temp);

            int colorInDouble = (int)Math.Abs(randomNumber);

            string hexValue = "'#000000'";


            switch (colorInDouble)
            {
                case 0://Black
                    hexValue = "'#000000'";
                    break;
                case 1://Blue
                    hexValue = "'#0000FF'";
                    break;
                case 2://Teal
                    hexValue = "'#00FFFF'";
                    break;
                case 3://Green
                    hexValue = "'#40FF00'";
                    break;
                case 4://Red
                    hexValue = "'#FF0000'";
                    break;
                case 5://Purple
                    hexValue = "'#FF00FF'";
                    break;
                case 6://Orange
                    hexValue = "'#FF8000'";
                    break;
                default://Yellow
                    hexValue = "'#FFFF00'";
                    break;
            }

            return hexValue;
        }

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
