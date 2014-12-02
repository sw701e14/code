using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Library;

namespace Webservice.Models.AvailableBikes
{
    public class availableBikes
    {
        public int count { get; set; }
        public List<availableBike> bikes { get; set; }

        public availableBikes()
        {
            Database context = new Database();

            bikes = new List<availableBike>();
            foreach (Tuple<Bike, GPSLocation> item in context.RunSession(session => session.GetAvailableBikes()))
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
    }
}