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
    [RoutePrefix("")]
    public class RootController : ApiController
    {
        public const int API_VERSION = 1;

        [Route("")]
        [HttpGet]
        [ResponseType(typeof(RootResource))]
        public HttpResponseMessage getResources()
        {
            List<RootResource.Resource> resources = new List<RootResource.Resource>();
            resources.Add(new RootResource.Resource() { href = "availablebikes" });

            return Request.CreateResponse(HttpStatusCode.OK,
                new RootResource() { apiVersion = API_VERSION, resources = resources });
        }
    }
}