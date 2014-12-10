using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models
{
    public static class Data
    {
        private const int IMMOBILE_MINUTES = 10;

        /// <summary>
        /// Gets a collection of all the available bikes.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>A collection of bikes and their location.</returns>
        public static Tuple<Bike, GPSLocation>[] GetAvailableBikes()
        {
            GPSData[] data;
            using (Database db = new Database())
                data = db.RunSession(session => Bike.GetLatestData(session));

            var immobileTimeSpan = new TimeSpan(0, IMMOBILE_MINUTES, 0);
            DateTime now = DateTime.Now;

            return (from b in data
                    where (now - b.QueryTime).CompareTo(immobileTimeSpan) > 0
                    select Tuple.Create(b.Bike, b.Location)).ToArray();
        }

        public static IEnumerable<bike> GetAllBikes()
        {
            GPSData[] data;
            using (Database db = new Database())
                data = db.RunSession(session => Bike.GetLatestData(session));

            Array.Sort(data, (x, y) => x.Bike.Id.CompareTo(y.Bike.Id));

            foreach (GPSData item in data)
            {
                bike b = new bike();
                b.id = item.Bike.Id;
                b.latitude = item.Location.Latitude;
                b.longitude = item.Location.Longitude;
                b.immobileSince = item.QueryTime;

                yield return b;
            }
        }

        public static IEnumerable<hotspot> GetAllHotspots()
        {
            Hotspot[] hotspots;
            using (Database context = new Database())
                hotspots = context.RunSession(s => Hotspot.LoadAllHotspots(s));

            foreach (var hs in hotspots)
            {
                hotspot tempHotspot = new hotspot();

                foreach (GPSLocation gpsLoc in hs.getDataPoints())
                {
                    coordinate tempCoordinate = new coordinate();
                    tempCoordinate.latitude = gpsLoc.Latitude;
                    tempCoordinate.longtitude = gpsLoc.Longitude;
                    tempHotspot.coordinates.Add(tempCoordinate);
                }
                yield return tempHotspot;
            }
        }
    }
}