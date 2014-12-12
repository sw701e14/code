using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.Available
{
    public class SingleAvailableBike
    {
        public long Id { get; set; }
        public GPSLocation Location { get; set; }

        public SingleAvailableBike(long bikeId) 
        {
            using (Database context = new Database())
            {
                var bike = Data.GetAvailableBikes().Where(x => x.Item1.Id == bikeId).FirstOrDefault();

                if (bike == null)
                    throw new NullReferenceException();

                Id = bike.Item1.Id;
                Location = bike.Item2;
            }
        }
    }
}