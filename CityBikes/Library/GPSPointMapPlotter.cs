using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;
using Google.Maps;
using System.IO;


namespace Library
{
    public static class GPSPointMapPlotter
    {
        static Database context = new Database();

        public static Uri PlotAllGPSPointImage()
        {
            //https://developers.google.com/maps/docum  entation/staticmaps/
            //Example: https://maps.googleapis.com/maps/api/staticmap?parameters

            AllBikesLocation allBikesLocation = new AllBikesLocation();
            List<Uri> mapLinks = new List<Uri>();

            string googleCore = "https://maps.googleapis.com/maps/api/staticmap?";
            string center = "center=aalborg,denmark&zoom=12&size=800x800";
            string link = googleCore + center;

            var locationList = from locations in context.gps_data
                            select locations;

            foreach (gps_data bikeLocation in locationList)
            {
                int bikeId = bikeLocation.bikeId;
                decimal latitude = bikeLocation.latitude;
                decimal longtitude = bikeLocation.longitude;
                string marker = "&markers=color:blue%7C";
                string label = "label:" + bikeId + ";" + bikeLocation.queried.ToString() + "%7C";
                string location = latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + longtitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                link = link + marker + label + location;
            }

            return new Uri(link);
        }


        public static void PlotAllGPSPointMap(string path)
        {
            //Center
            //57.0338295,9.9277601
            //Zoom
            //12


            /* Test Data */
            /*
                57.01168540 , 9.99205080
                57.01200010 , 9.99196300
                57.01231290 , 9.99199550
            */

            var locationList = from locations in context.gps_data
                               select locations;

            StreamWriter streamWriter = new StreamWriter(path);
            string apiKey = "AIzaSyDY0kkJiTPVd2U7aTOAwhc9ySH6oHxOIYM";
            string centerLatitude = "57.0338295";
            string centerLongtitude = "9.9277601";
            string zoom = "12";
            gps_data previousData = null;
            string lineString = "";

            createHTML(apiKey, centerLatitude, centerLongtitude, zoom, streamWriter);

            foreach (gps_data bikeLocation in locationList)
            {
                streamWriter.WriteLine("placeMarker(new google.maps.LatLng(" + bikeLocation.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),'" + bikeLocation.bikeId + "','" + bikeLocation.queried.ToString() + "');");

                if (!(previousData == null))
                {
                    if (previousData.bikeId == bikeLocation.bikeId)
                    {
                        if (!(lineString.StartsWith("var myLine" + bikeLocation.bikeId + "=[")))
                        {
                            lineString = "var myLine" + bikeLocation.bikeId + "=[";
                        }

                        lineString = lineString + "new google.maps.LatLng(" + bikeLocation.latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + bikeLocation.longitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "),";
                    }
                    else
                    {
                        createLineForBike(streamWriter, lineString, bikeLocation.bikeId);
                        lineString = "";
                    }
                }

                previousData = bikeLocation;
            }

            createPlaceMarkerMethod(streamWriter);

            finalizeHTML(streamWriter);

            streamWriter.Close();
        }


        private static void createHTML(string apiKey, string centerLatitude, string centerLongtitude, string zoom, StreamWriter streamWriter)
        {
            streamWriter.WriteLine("<!DOCTYPE html>");
            streamWriter.WriteLine("<html>");
            streamWriter.WriteLine("<head>");
            streamWriter.WriteLine("<script");
            streamWriter.WriteLine("src=\"http://maps.googleapis.com/maps/api/js?key=" + apiKey + "&sensor=false\">");
            streamWriter.WriteLine("</script>");
            streamWriter.WriteLine("<script>");
            streamWriter.WriteLine("var map;");
            streamWriter.WriteLine("var myCenter=new google.maps.LatLng(" + centerLatitude + "," + centerLongtitude + ");");
            streamWriter.WriteLine("function initialize(){");
            streamWriter.WriteLine("var mapProp = {");
            streamWriter.WriteLine("center:myCenter,");
            streamWriter.WriteLine("zoom:" + zoom + ",");
            streamWriter.WriteLine("mapTypeId:google.maps.MapTypeId.ROADMAP};");
            streamWriter.WriteLine("map = new google.maps.Map(document.getElementById(\"googleMap\"),mapProp);");
        }

        private static void createLineForBike(StreamWriter streamWriter, string lineString, int bikeID)
        {
            streamWriter.WriteLine(lineString + "]");

            streamWriter.WriteLine("var completeLine" + (bikeID - 1) + "=new google.maps.Polyline({");
            streamWriter.WriteLine("path:myLine" + (bikeID - 1) + ",");
            streamWriter.WriteLine("strokeColor:\"#0000FF\",");
            streamWriter.WriteLine("strokeOpacity:0.8,");
            streamWriter.WriteLine("strokeWeight:2});");
            streamWriter.WriteLine("completeLine" + (bikeID - 1) + ".setMap(map);");
        }


        private static void createPlaceMarkerMethod(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("}");

            streamWriter.WriteLine("function placeMarker(location,bikeID, date) {");
            streamWriter.WriteLine("var marker = new google.maps.Marker({");
            streamWriter.WriteLine("position: location,");
            streamWriter.WriteLine("map: map,});");
            streamWriter.WriteLine("var infowindow = new google.maps.InfoWindow({");
            streamWriter.WriteLine("content: 'BikeID: ' + bikeID + '<br>Date: ' + date+ '<br>Latitude: ' + location.lat() + '<br>Longitude: ' + location.lng()" + "});");
            streamWriter.WriteLine("google.maps.event.addListener(marker, 'click', function() {");
            streamWriter.WriteLine("infowindow.open(map,marker);" + "});}");
        }

        private static void finalizeHTML(StreamWriter streamWriter)
        {
            streamWriter.WriteLine("google.maps.event.addDomListener(window, 'load', initialize);");
            streamWriter.WriteLine("</script>");
            streamWriter.WriteLine("</head>");
            streamWriter.WriteLine("<body>");
            streamWriter.WriteLine("<div id=\"googleMap\" style=\"width:600px;height:600px;\"></div>");
            streamWriter.WriteLine("</body>");
            streamWriter.WriteLine("</html>");
        }

    }
}
