using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Library;
using Webservice.Models.Hotspots;

namespace Webservice.Controllers
{
    /// <summary>
    /// Publicly available methods for getting available bikes' locations.
    /// </summary>
    [RoutePrefix("hotspots")]
    public class HotspotsController : ApiController
    {
        /// <summary>
        /// Get list of available bikes.
        /// </summary>
        /// <returns>The bikes.</returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(hotspots))]
        public HttpResponseMessage getAll()
        {
            Database context = new Database();

            hotspots hotspotResources = new hotspots();
            int hotspotsCount = 0;
            foreach (Hotspot item in context.RunSession(session => session.GetAllHotspots()))
            {
                hotspotsCount++;
                
               
                
               
               
            }
            hotspotResources.count = hotspotsCount;

            return Request.CreateResponse(HttpStatusCode.OK, hotspotResources);
        }
    }
}