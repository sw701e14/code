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
        public int count { get; set; }
        public List<bike> bikes { get; set; }

        public allBikes()
        {
            bikes = new List<bike>();
            foreach (var b in Data.GetAllBikes().ToList())
            {
                bikes.Add(b);
                count++;
            }
            bikes = bikes.OrderBy(x => x.id).ToList();
        }
        
    }
}