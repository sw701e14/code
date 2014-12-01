﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Library;
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
        [ResponseType(typeof(bikes))]
        public HttpResponseMessage getAll()
        {
            Database context = new Database();

            Tuple<Bike, DateTime, bool>[] immobileSinceTimes = context.RunSession(session => Library.BikeStandstill.GetBikesImmobile(session));

            bikes bikeResources = new bikes();
            int bikeCount = 0;
            foreach (Tuple<Bike, GPSLocation> item in context.RunSession(session => session.GetBikeLocations()))
            {
                bikeCount++;
                bikeResources.bikeList.Add(new Webservice.Models.AllBikes.bikes.bike()
                {
                    id = item.Item1.Id,
                    latitude = item.Item2.Latitude.ToString(),
                    longtitude = item.Item2.Longitude.ToString(),
                    immobileSince = immobileSinceTimes.Where(x => x.Item1.Id == item.Item1.Id).FirstOrDefault().Item2.ToString()
                });
            }
            bikeResources.count = bikeCount;
            bikeResources.bikeList = bikeResources.bikeList.OrderBy(x => x.id).ToList();

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
            Database context = new Database();
            var bikeLocation = context.RunSession(session => session.GetBikeLocation(bikeId));

            Tuple<Bike, DateTime, bool>[] immobileSinceTimes = context.RunSession(session => Library.BikeStandstill.GetBikesImmobile(session));

            if (bikeLocation == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bike bikeResource = new bike() { id = bikeId, latitude = bikeLocation.Latitude, longitude = bikeLocation.Longitude, immobileSince = immobileSinceTimes.Where(x => x.Item1.Id == bikeId).FirstOrDefault().Item2 };

            return Request.CreateResponse(HttpStatusCode.OK, bikeResource);
        }
    }
}