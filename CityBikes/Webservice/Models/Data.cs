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
        public static Tuple<Shared.DTO.Bike, GPSLocation>[] GetAvailableBikes()
        {
            using (Database db = new Database())
            {
                Dictionary<Shared.DTO.Bike, GPSLocation> positions = db.RunSession(session => session.GetBikeLocations().ToDictionary(x => x.Item1, x => x.Item2));
                Dictionary<Shared.DTO.Bike, DateTime> immobile = db.RunSession(session => session.GetBikesImmobile().ToDictionary(x => x.Item1, x => x.Item2));

                var immobileTimeSpan = new TimeSpan(0, IMMOBILE_MINUTES, 0);
                DateTime now = DateTime.Now;

                return (from p in immobile
                        where (now - p.Value).CompareTo(immobileTimeSpan) > 0
                        select Tuple.Create(p.Key, positions[p.Key])).ToArray();
            }
        }

        public static IEnumerable<Bike> GetAllBikes()
        {
            using (Database context = new Database())
            {
                Tuple<Shared.DTO.Bike, DateTime, bool>[] immobileSinceTimes = context.RunSession(session => session.GetBikesImmobile());

                foreach (Tuple<Shared.DTO.Bike, GPSLocation> item in context.RunSession(session => session.GetBikeLocations()))
                {
                    Bike b = new Bike();
                    b.Id = item.Item1.Id;
                    b.Latitude = item.Item2.Latitude;
                    b.Longitude = item.Item2.Longitude;
                    b.ImmobileSince = immobileSinceTimes.Where(x => x.Item1.Id == item.Item1.Id).FirstOrDefault().Item2;

                    yield return b;
                }
            }
        }

        public static IEnumerable<Webservice.Models.Hotspot> GetAllHotspots()
        {
            using (Database context = new Database())
            {
                foreach (Shared.DTO.Hotspot item in context.RunSession(session => session.GetAllHotspots()))
                {
                    Webservice.Models.Hotspot tempHotspot = new Webservice.Models.Hotspot();

                    foreach (GPSLocation gpsLoc in item.getDataPoints())
                    {
                        Coordinate tempCoordinate = new Coordinate();
                        tempCoordinate.Latitude = gpsLoc.Latitude;
                        tempCoordinate.Longtitude = gpsLoc.Longitude;
                        tempHotspot.Coordinates.Add(tempCoordinate);
                    }
                    yield return tempHotspot;
                }
            }
        }

        public static IEnumerable<Prediction> GetPredictions()
        {
            throw new NotImplementedException();
        }
    }
}