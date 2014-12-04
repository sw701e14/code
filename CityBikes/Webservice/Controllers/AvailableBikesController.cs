using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Library;
using Webservice.Models.AvailableBikes;
using Shared.DAL;
using Shared.DTO;

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
            int bikeCount = 0;
            foreach (Tuple<Bike, GPSLocation> item in Webservice.Models.AvailableBikes.AvailableBikes.GetAvailableBikes())
            {
                bikeCount++;
                bikeResources.bikes.Add(new Webservice.Models.AvailableBikes.AvailableBikes.availableBike() { href = item.Item1.Id.ToString() } );
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
            var bikeLocation = Webservice.Models.AvailableBikes.AvailableBikes.GetAvailableBikes().Where(x => x.Item1.Id == bikeId).FirstOrDefault();


            if (bikeLocation == null)
                throw new HttpResponseException(HttpStatusCode.NotFound);

            AvailableBike bikeResource = new AvailableBike() { id = bikeLocation.Item1.Id, latitude = bikeLocation.Item2.Latitude, longitude = bikeLocation.Item2.Longitude };

            return Request.CreateResponse(HttpStatusCode.OK, bikeResource);
        }
    }
}