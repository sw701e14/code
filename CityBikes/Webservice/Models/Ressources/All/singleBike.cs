using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.All
{
    public class SingleBike
    {
        public long Id { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime ImmobileSince { get; set; }

        public SingleBike(long bikeId) 
        {
            var bike = Data.GetAllBikes().Where(x => x.Id == bikeId).FirstOrDefault();

            if (bike == null)
                throw new NullReferenceException();

            Latitude = bike.Latitude;
            Longitude = bike.Longitude;
            Id = bike.Id;
            ImmobileSince = bike.ImmobileSince;
        }
    }
}