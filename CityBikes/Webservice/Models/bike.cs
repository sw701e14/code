using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models
{
    public class bike
    {
        public long id { get; set; }
        public decimal latitude { get; set; }
        public decimal longitude { get; set; }

        public DateTime immobileSince { get; set; }

        public bike() { }
    }
}