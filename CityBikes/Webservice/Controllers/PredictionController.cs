using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Webservice.Models.Ressources.Predictions;

namespace Webservice.Controllers
{
    [RoutePrefix("predictions")]
    public class PredictionController : ApiController
    {

        [Route("")]
        [HttpGet]
        [ResponseType(typeof(AllPredictions))]
        public HttpResponseMessage allPredictions()
        {
            AllPredictions allPredictions = new AllPredictions();

            return Request.CreateResponse(HttpStatusCode.OK, allPredictions);
        }
    }
}