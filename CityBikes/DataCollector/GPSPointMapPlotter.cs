using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library;
using System.IO;

namespace DataLoading.DataCollector
{
    /// <summary>
    /// Provides methods that creates a Google Map with plotted in GPS locations.
    /// Maps are saved to working directory (default: /run/Debug/)
    /// </summary>
    public class GPSPointMapPlotter
    {
        //Setup for Google Maps API
        private const string API_KEY = "AIzaSyDY0kkJiTPVd2U7aTOAwhc9ySH6oHxOIYM";
        private static readonly GPSLocation CENTER = new GPSLocation(57.0338295m, 9.9277601m);
        private const string ZOOM = "12";
        private const string MAP_WIDTH = "600";
        private const string MAP_HEIGHT = "600";

        private StringBuilder sb;

        private GPSPointMapPlotter()
        {
            this.sb = new StringBuilder();
        }

        /// <summary>
        /// Plots ALL <see cref="GPSData"/> from database to a Google Map and connects same-bike-id-points with lines.
        /// Saves the map as a HTML-file in working directory.
        /// </summary>
        /// <param name="session">The <see cref="Database.DatabaseSession"/> used to load gps data.</param>
        public static void SaveMapAsHtml(Database.DatabaseSession session)
        {
            if (session == null)
                throw new ArgumentNullException();

            GPSData[] data = session.ExecuteRead("SELECT * FROM citybike_test.gps_data").Select(r => r.GetGPSData(1)).ToArray();

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
                throw new ArgumentNullException("selectedGPSPoints");

            GPSPointMapPlotter plotter = new GPSPointMapPlotter();
            plotter.generateHTML(selectedGPSPoints.ToArray());
            File.WriteAllText("map.html", plotter.sb.ToString());
        }

        private void appendLocation(GPSLocation location)
        {
            sb.AppendFormat("{0}, {1}",
                location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        private string formatLocation(GPSLocation location)
        {
            return string.Format("{0}, {1}",
                location.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                location.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }
        
        private void generateHTML(GPSData[] data)
        {
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html>");
            sb.AppendLine("<head>");

            sb.AppendLine("<script src=\"http://maps.googleapis.com/maps/api/js?key=" + API_KEY + "&sensor=false\"></script>");

            sb.AppendLine("<script>");
            sb.AppendLine("var map;");
            sb.AppendLine("var myCenter=new google.maps.LatLng(" + formatLocation(CENTER) + ");");

            sb.AppendLine("function initialize(){");
            sb.AppendLine("var mapProp = {");
            sb.AppendLine("center:myCenter,");
            sb.AppendLine("zoom:" + ZOOM + ",");
            sb.AppendLine("mapTypeId:google.maps.MapTypeId.ROADMAP};");
            sb.AppendLine("map = new google.maps.Map(document.getElementById(\"googleMap\"),mapProp);");

            writeHTMLPointsAndLines(data);

            sb.AppendLine("function addMarker(location,bikeID, date) {");
            sb.AppendLine("var marker = new google.maps.Marker({");
            sb.AppendLine("position: location,");
            sb.AppendLine("map: map,");
            sb.AppendLine("icon: 'http://maps.google.com/mapfiles/ms/icons/blue-dot.png'});");
            sb.AppendLine("var infowindow = new google.maps.InfoWindow({");
            sb.AppendLine("content: 'BikeID: ' + bikeID + '<br>Date: ' + date+ '<br>Latitude: ' + location.lat() + '<br>Longitude: ' + location.lng()" + "});");
            sb.AppendLine("google.maps.event.addListener(marker, 'click', function() {");
            sb.AppendLine("infowindow.open(map,marker);" + "});}");

            sb.AppendLine("google.maps.event.addDomListener(window, 'load', initialize);");
            sb.AppendLine("</script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<div id=\"googleMap\" style=\"width:" + MAP_WIDTH + "px;height:" + MAP_HEIGHT + "px;\"></div>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
        }

        private void writeHTMLPointsAndLines(IEnumerable<GPSData> locationList)
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
                    }
                    else
                    {
                        //Writes method and settings for creating lines for this bikeID.
                        result = result + writeHTMLLine(lineString, bikeLocation.Bike.Id - 1, bikeLocation.QueryTime);
                        lineString = "var myLine" + bikeLocation.Bike.Id + "=[";
                    }
                    lineString = lineString + "new google.maps.LatLng(" + formatLocation(bikeLocation.Location) + "),";
                }
                else
                {
                    //Writes line variable (eg. var myLine1) and add connection points for the first line.
                    if (!(lineString.StartsWith("var myLine" + bikeLocation.Bike.Id + "=[")))
                    {
                        lineString = "var myLine" + bikeLocation.Bike.Id + "=[";
                    }
                    lineString = lineString + "new google.maps.LatLng(" + formatLocation(bikeLocation.Location) + "),";
                }

                //Writes all markers.
                result = result + "addMarker(new google.maps.LatLng(" + formatLocation(bikeLocation.Location) + "),'" + bikeLocation.Bike.Id + "','" + bikeLocation.QueryTime.ToString() + "');" + System.Environment.NewLine;

                if (bikeLocation.Equals(lastInList))
                {
                    //Writes method and settings for creating lines for the last bikeID.
                    result = result + writeHTMLLine(lineString, bikeLocation.Bike.Id, bikeLocation.QueryTime);
                }

                previousData = bikeLocation;
            }

            sb.AppendLine(result + "}");
        }

        private string writeHTMLLine(string lineDataAsString, long bikeID, DateTime queried)
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
    }
}
