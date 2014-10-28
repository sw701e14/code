using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;
using Google.Maps;


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


    }
}
