using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Library;

namespace Webservice.Models.AllBikes
{
    public class bike
    {
        public long id { get; set; }
        public GPSLocation location { get; set; }
        public DateTime immobileSince { get; set; }

        public bike(long bikeId) 
        {
            Database context = new Database();
            var bikeLocation = context.RunSession(session => session.GetBikeLocation(bikeId));

            if (bikeLocation == null)
                throw new NullReferenceException();

            location = bikeLocation;
            id = bikeId;

            Tuple<Bike, DateTime, bool>[] immobileSinceTimes = context.RunSession(session => session.GetBikesImmobile());

            immobileSince = immobileSinceTimes.Where(x => x.Item1.Id == id).FirstOrDefault().Item2;
        }
    }
}