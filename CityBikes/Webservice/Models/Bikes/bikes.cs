using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.AllBikes
{
    public class allBikes
    {
        public int count { get; set; }
        public List<Webservice.Models.AllBikes.allBikes.bike> bikes { get; set; }

        public allBikes()
        {
            foreach (var b in GetAllBikes().ToList())
            {
                bikes.Add(b);
                count++;
            }
            bikes = bikes.OrderBy(x => x.id).ToList();
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

        public class bike
        {
            public long id { get; set; }
            public decimal latitude { get; set; }
            public decimal longitude { get; set; }

            public DateTime immobileSince { get; set; }

            public bike() { }
        }
    }
}