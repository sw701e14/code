using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.AvailableBikes
{
    public class availableBikes
    {
        public const int IMMOBILE_MINUTES = 10;

        public int count { get; set; }
        public List<availableBike> bikes { get; set; }

        public availableBikes()
        {
            bikes = new List<availableBike>();
            foreach (Tuple<Bike, GPSLocation> item in GetAvailableBikes())
            {
                count++;
                bikes.Add(new Webservice.Models.AvailableBikes.availableBikes.availableBike() { href = item.Item1.Id.ToString() });
            }
        }

        public class availableBike
        {
            public string href { get; set; }

            public availableBike() { }
        }

        /// <summary>
        /// Gets a collection of all the available bikes.
        /// </summary>
        /// <param name="session">A <see cref="Database.DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>A collection of bikes and their location.</returns>
        public static Tuple<Bike, GPSLocation>[] GetAvailableBikes()
        {
            using (Database db = new Database())
            {
                Dictionary<Bike, GPSLocation> positions = db.RunSession(session=>session.GetBikeLocations().ToDictionary(x => x.Item1, x => x.Item2));
                Dictionary<Bike, DateTime> immobile = db.RunSession(session=>session.GetBikesImmobile().ToDictionary(x => x.Item1, x => x.Item2));

                var immobileTimeSpan = new TimeSpan(0, IMMOBILE_MINUTES, 0);
                DateTime now = DateTime.Now;

                return (from p in immobile
                        where (now - p.Value).CompareTo(immobileTimeSpan) > 0
                        select Tuple.Create(p.Key, positions[p.Key])).ToArray();
            }
        }
    }
}