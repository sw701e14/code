﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Webservice.Models.AllBikes;

namespace Webservice.Controllers
{
    /// <summary>
    /// Publicly available methods for getting all bikes' locations.
    /// </summary>
    [RoutePrefix("bikes")]
    public class BikesController : ApiController
    {
        /// <summary>
        /// Get list of all bikes.
        /// </summary>
        /// <returns>The bikes.</returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(allBikes))]
        public HttpResponseMessage getAll()
        {
            allBikes bikeResources = new allBikes();

            return Request.CreateResponse(HttpStatusCode.OK, bikeResources);
        }

        /// <summary>
        /// Get bike with bikeId.
        /// </summary>
        /// <param name="bikeId">The bikeId.</param>
        /// <returns>The bike.</returns>
        [Route("{bikeId}")]
        [HttpGet]
        public HttpResponseMessage get(long bikeId)
        {
            singleBike bikeResource;
            try
            {
                bikeResource = new singleBike(bikeId);
            }
            catch (NullReferenceException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, bikeResource);
        }
    }
}