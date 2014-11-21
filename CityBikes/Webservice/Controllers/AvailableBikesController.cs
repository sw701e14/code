﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Library;
using Webservice.Models.AvailableBikes;

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
        [ResponseType(typeof(ListAvailableBikes))]
        public HttpResponseMessage getAll()
        {
            Database context = new Database();

            ListAvailableBikes bikeResources = new ListAvailableBikes();
            int bikeCount = 0;
            foreach (Tuple<Bike, GPSLocation> item in context.RunSession(session => session.GetAvailableBikes()))
            {
                bikeCount++;
                bikeResources.bikes.Add(new Webservice.Models.AvailableBikes.bike() { id = item.Item1.Id, href = item.Item1.ToString() } );
            }
            bikeResources.count = bikeCount;

            return Request.CreateResponse(HttpStatusCode.OK, bikeResources);
        }

        /// <summary>
        /// Get available bike.
        /// </summary>
        /// <param name="bikeId">The bikeId.</param>
        /// <returns>The bike.</returns>
        [Route("{bikeId}")]
        [HttpGet]
        public HttpResponseMessage get(long bikeId)
        {
            Database context = new Database();
            var bikeLocation = context.RunSession(session => session.GetAvailableBikes()).Where(x => x.Item1.Id == bikeId).FirstOrDefault();


            if (bikeLocation == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            BikeResource bikeResource = new BikeResource() { bike = bikeLocation.Item1, latitude = bikeLocation.Item2.Latitude, longitude = bikeLocation.Item2.Longitude };

            return Request.CreateResponse(HttpStatusCode.OK, bikeResource);
        }
    }
}