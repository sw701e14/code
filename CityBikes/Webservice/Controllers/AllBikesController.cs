using System;
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
    /// Publicly available methods for getting available bikes' locations.
    /// </summary>
    [RoutePrefix("allbikes")]
    public class AllBikesController : ApiController
    {
        /// <summary>
        /// Get list of available bikes.
        /// </summary>
        /// <returns>The bikes.</returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(allBikes))]
        public HttpResponseMessage getAll()
        {
            Database context = new Database();

            Tuple<Bike, DateTime, bool>[] immobileSinceTimes = context.RunSession(session => Library.BikeStandstill.GetBikesImmobile(session));

            allBikes bikeResources = new allBikes();
            int bikeCount = 0;
            foreach (Tuple<Bike, GPSLocation> item in context.RunSession(session => session.GetBikeLocations()))
            {
                bikeCount++;
                bikeResources.bikes.Add(new Webservice.Models.AllBikes.allBikes.bike()
                {
                    id = item.Item1.Id.ToString(),
                    latitude = item.Item2.Latitude.ToString(),
                    longtitude = item.Item2.Longitude.ToString(),
                    immobileSince = immobileSinceTimes.Where(x => x.Item1.Id == item.Item1.Id).FirstOrDefault().Item2.ToString()
                });
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
            var bikeLocation = context.RunSession(session => session.GetBikeLocation(bikeId));

            Tuple<Bike, DateTime, bool>[] immobileSinceTimes = context.RunSession(session => Library.BikeStandstill.GetBikesImmobile(session));

            if (bikeLocation == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bike bikeResource = new bike() { id = bikeId, latitude = bikeLocation.Latitude, longitude = bikeLocation.Longitude, immobileSince = immobileSinceTimes.Where(x => x.Item1.Id == bikeId).FirstOrDefault().Item2 };

            return Request.CreateResponse(HttpStatusCode.OK, bikeResource);
        }
    }
}