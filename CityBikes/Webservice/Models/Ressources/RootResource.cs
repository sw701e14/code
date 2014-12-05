using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Webservice.Models.Root
{
    public class RootResource
    {
        public int apiVersion { get; set; }
        public List<Resource> resources { get; set; }

        public RootResource() { }

        public class Resource
        {
            public string href { get; set; }

            public Resource () { }
        }
    }
}