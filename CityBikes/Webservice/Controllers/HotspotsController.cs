using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Webservice.Models.Hotspots;

namespace Webservice.Controllers
{
    /// <summary>
    /// Publicly available methods for getting all hotspots.
    /// </summary>
    [RoutePrefix("hotspots")]
    public class HotspotsController : ApiController
    {
        /// <summary>
        /// Get list of hotspots.
        /// </summary>
        /// <returns>The hotspots.</returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(allHotspots))]
        public HttpResponseMessage getAll()
        {
            allHotspots hotspotResources = new allHotspots();

            return Request.CreateResponse(HttpStatusCode.OK, hotspotResources);
        }
    }
}