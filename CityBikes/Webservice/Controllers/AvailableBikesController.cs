﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Webservice.Models.Available;

namespace Webservice.Controllers
{
    /// <summary>
    /// Publicly available methods for getting available bikes' locations.
    /// </summary>
    [RoutePrefix("availablebikes")]
    public class AvailableBikesController : ApiController
    {
        /// <summary>
        /// Get list of available bikes.
        /// </summary>
        /// <returns>The bikes.</returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(AvailableBikes))]
        public HttpResponseMessage getAll()
        {
            AvailableBikes bikeResources = new AvailableBikes();

            return Request.CreateResponse(HttpStatusCode.OK, bikeResources);
        }

        /// <summary>
        /// Get available bike.
        /// </summary>
        /// <param name="bikeId">The bikeId.</param>
        /// <returns>The bike.</returns>
        [Route("{bikeId}")]
        [HttpGet]
        [ResponseType(typeof(SingleAvailableBike))]
        public HttpResponseMessage get(long bikeId)
        {
            SingleAvailableBike bikeResource;
            try
            {
                bikeResource = new SingleAvailableBike(bikeId);
            }
            catch (NullReferenceException)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, bikeResource);
        }
    }
}