using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.All
{
    public class AllBikes
    {
        public int Count { get { return Bikes.Count(); } }
        public List<Bike> Bikes { get; set; }

        public AllBikes()
        {
            Bikes = new List<Bike>();

            Bikes.AddRange(Data.GetAllBikes());

            Bikes = Bikes.OrderBy(x => x.Id).ToList();
        }

    }
}