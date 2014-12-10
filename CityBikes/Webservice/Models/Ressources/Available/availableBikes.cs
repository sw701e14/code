using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.Available
{
    public class availableBikes
    {
        public int count { get { return bikes.Count(); } }
        public List<availableBike> bikes { get; set; }

        public availableBikes()
        {
            bikes = new List<availableBike>();
            foreach (Tuple<Bike, GPSLocation> item in Data.GetAvailableBikes())
            {
                bikes.Add(new availableBike() { href = item.Item1.Id.ToString() });
            }
        }
    }
}