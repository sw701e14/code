using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Webservice.Models.Root;

namespace Webservice.Controllers
{
    /// <summary>
    /// API overview
    /// </summary>
    [RoutePrefix("")]
    public class RootController : ApiController
    {
        private const int API_VERSION = 1;

        /// <summary>
        /// The API overview.
        /// </summary>
        /// <returns>API version and list of resources.</returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(RootResource))]
        public HttpResponseMessage getRoot()
        {
            List<RootResource.Resource> resources = new List<RootResource.Resource>();
            resources.Add(new RootResource.Resource() { href = "availablebikes" });
            resources.Add(new RootResource.Resource() { href = "hotspots" });
            resources.Add(new RootResource.Resource() { href = "predictions" });


            return Request.CreateResponse(HttpStatusCode.OK,
                new RootResource() { apiVersion = API_VERSION, resources = resources });
        }
    }
}