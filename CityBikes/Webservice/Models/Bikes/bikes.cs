using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Library;

namespace Webservice.Models.AllBikes
{
    public class allBikes
    {
        public int count { get; set; }
        public List<Webservice.Models.AllBikes.allBikes.bike> bikes { get; set; }

        public allBikes()
        {
            bikes = new List<Webservice.Models.AllBikes.allBikes.bike>();
            Database context = new Database();

            Tuple<Bike, DateTime, bool>[] immobileSinceTimes = context.RunSession(session => session.GetBikesImmobile());
            
            foreach (Tuple<Bike, GPSLocation> item in context.RunSession(session => session.GetBikeLocations()))
            {
                count++;
                bikes.Add(new Webservice.Models.AllBikes.allBikes.bike()
                {
                    id = item.Item1.Id,
                    latitude = item.Item2.Latitude,
                    longtitude = item.Item2.Longitude,
                    immobileSince = immobileSinceTimes.Where(x => x.Item1.Id == item.Item1.Id).FirstOrDefault().Item2
                });
            }
            bikes = bikes.OrderBy(x => x.id).ToList();
        }

        public class bike
        {
            public long id { get; set; }
            public decimal latitude { get; set; }
            public decimal longtitude { get; set; }

            public DateTime immobileSince { get; set; }

            public bike() { }
        }
    }
}