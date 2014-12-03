using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Library;

namespace Webservice.Models.AvailableBikes
{
    public class availableBike
    {
        public long id { get; set; }
        public GPSLocation location { get; set; }

        public availableBike(long bikeId) 
        {
            Database context = new Database();
            var bike = context.RunSession(session => session.GetAvailableBikes()).Where(x => x.Item1.Id == bikeId).FirstOrDefault();

            if (bike == null)
                throw new NullReferenceException();

            id = bike.Item1.Id;
            location = bike.Item2;
        }
    }
}