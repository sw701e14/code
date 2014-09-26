using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library.GeneratedDatabaseModel
{
    public partial class gps_data
    {
        public GPSLocation Location
        {
            get { return new GPSLocation(this.latitude, this.longitude); }
            set { this.latitude = value.Latitude; this.longitude = value.Longitude; }
        }
    }
}
