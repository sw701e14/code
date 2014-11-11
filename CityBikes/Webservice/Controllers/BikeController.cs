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
    /// <summary>
    /// Publicly available methods for getting bike locations and predictions.
    /// </summary>
    [RoutePrefix("bikes")]
    public class BikeController : ApiController
    {
        /// <summary>
        /// Get all available bike locations.
        /// </summary>
        /// <returns>The bike locations.</returns>
        [Route("")]
        [HttpGet]
        public Tuple<long, GPSLocation>[] getAll()
        {
            Database context = new Database();

            return AvailableBikes.GetAvailableBikes(context).ToArray();
        }
    }
}