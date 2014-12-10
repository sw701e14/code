using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Webservice.Models.Ressources.Prediction;

namespace Webservice.Controllers
{
    [RoutePrefix("predictions")]
    public class PredictionController : ApiController
    {

        [Route("")]
        [HttpGet]
        [ResponseType(typeof(predictions))]
        public HttpResponseMessage allPredictions()
        {
            predictions allPredictions = new predictions();

            return Request.CreateResponse(HttpStatusCode.OK, allPredictions);
        }
    }
}