﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;
using Webservice.Models.Ressources;

namespace Webservice.Models.Available
{
    public class AvailableBikes
    {
        public int Count { get { return Bikes.Count(); } }
        public List<AvailableBike> Bikes { get; set; }

        public AvailableBikes()
        {
            Bikes = new List<AvailableBike>();
            foreach (Tuple<Shared.DTO.Bike, GPSLocation> item in Data.GetAvailableBikes())
            {
                Bikes.Add(new AvailableBike() { Href = ResourceAdressing.GetAvailableBike(item.Item1.Id) });
            }
        }
    }
}