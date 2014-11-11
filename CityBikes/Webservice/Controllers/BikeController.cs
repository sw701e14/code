using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Library;
using Library.GeneratedDatabaseModel;

namespace Webservice.Controllers
{
    [RoutePrefix("bikes")]
    public class BikeController : ApiController
    {
        [Route("")]
        [HttpGet]
        public Tuple<long, GPSLocation>[] getAll()
        {
            Database context = new Database();

            return AvailableBikes.GetAvailableBikes(context).ToArray();
        }
    }
}