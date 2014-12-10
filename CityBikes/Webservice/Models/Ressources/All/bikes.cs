using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models.All
{
    public class allBikes
    {
        public int count { get { return bikes.Count(); } }
        public List<bike> bikes { get; set; }

        public allBikes()
        {
            bikes = new List<bike>();

            bikes.AddRange(Data.GetAllBikes());

            bikes = bikes.OrderBy(x => x.id).ToList();
        }

    }
}