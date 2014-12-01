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

            allBikes bikeResources = new allBikes();
            int bikeCount = 0;
            foreach (Tuple<Bike, GPSLocation> item in context.RunSession(session => session.GetBikeLocations()))
            {
                bikeCount++;
                bikeResources.bikes.Add(new Webservice.Models.AllBikes.allBikes.bike() { href = item.Item1.Id.ToString() } );
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

            if (bikeLocation == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            bike bikeResource = new bike() { id = bikeId, latitude = bikeLocation.Latitude, longitude = bikeLocation.Longitude };

            return Request.CreateResponse(HttpStatusCode.OK, bikeResource);
        }
    }
}