using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.All
{
    public class singleBike
    {
        public long id { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }
        public DateTime immobileSince { get; set; }

        public singleBike(long bikeId) 
        {
            var bike = Data.GetAllBikes().Where(x => x.id == bikeId).FirstOrDefault();

            if (bike == null)
                throw new NullReferenceException();

            latitude = bike.latitude;
            longitude = bike.longitude;
            id = bike.id;
            immobileSince = bike.immobileSince;
        }
    }
}