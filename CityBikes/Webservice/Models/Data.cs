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
        public const int IMMOBILE_MINUTES = 10;

        /// <summary>
        /// Gets a collection of all the available bikes.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>A collection of bikes and their location.</returns>
        public static Tuple<Bike, GPSLocation>[] GetAvailableBikes()
        {
            using (Database db = new Database())
            {
                Dictionary<Bike, GPSLocation> positions = db.RunSession(session => session.GetBikeLocations().ToDictionary(x => x.Item1, x => x.Item2));
                Dictionary<Bike, DateTime> immobile = db.RunSession(session => session.GetBikesImmobile().ToDictionary(x => x.Item1, x => x.Item2));

                var immobileTimeSpan = new TimeSpan(0, IMMOBILE_MINUTES, 0);
                DateTime now = DateTime.Now;

                return (from p in immobile
                        where (now - p.Value).CompareTo(immobileTimeSpan) > 0
                        select Tuple.Create(p.Key, positions[p.Key])).ToArray();
            }
        }

        public static IEnumerable<bike> GetAllBikes()
        {
            using (Database context = new Database())
            {
                Tuple<Bike, DateTime, bool>[] immobileSinceTimes = context.RunSession(session => session.GetBikesImmobile());

                foreach (Tuple<Bike, GPSLocation> item in context.RunSession(session => session.GetBikeLocations()))
                {
                    bike b = new bike();
                    b.id = item.Item1.Id;
                    b.latitude = item.Item2.Latitude;
                    b.longitude = item.Item2.Longitude;
                    b.immobileSince = immobileSinceTimes.Where(x => x.Item1.Id == item.Item1.Id).FirstOrDefault().Item2;

                    yield return b;
                }
            }
        }

        public static IEnumerable<hotspot> GetAllHotspots()
        {
            using (Database context = new Database())
            {
                foreach (Hotspot item in context.RunSession(session => session.GetAllHotspots()))
                {
                    hotspot tempHotspot = new hotspot();

                    foreach (GPSLocation gpsLoc in item.getDataPoints())
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
}