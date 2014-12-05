using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.AvailableBikes
{
    public class singleAvailableBike
    {
        public long id { get; set; }
        public GPSLocation location { get; set; }

        public singleAvailableBike(long bikeId) 
        {
            using (Database context = new Database())
            {
                var bike = availableBikes.GetAvailableBikes().Where(x => x.Item1.Id == bikeId).FirstOrDefault();

                if (bike == null)
                    throw new NullReferenceException();

                id = bike.Item1.Id;
                location = bike.Item2;
            }
        }
    }
}