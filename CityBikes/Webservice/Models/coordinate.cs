using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class Coordinate
    {
        public decimal Latitude { get; private set; }
        public decimal Longtitude { get; private set; }

        public Coordinate(decimal latitude, decimal longitude) 
        {
            this.Latitude = latitude;
            this.Longtitude = longitude;
        }

        public static Coordinate ConvertFromLocation(Shared.DTO.GPSLocation location)
        {
            return new Coordinate(location.Latitude, location.Longitude);
        }
    }
}