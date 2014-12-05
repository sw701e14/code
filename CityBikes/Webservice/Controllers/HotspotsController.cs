using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Library;
using Webservice.Models.Hotspots;

namespace Webservice.Controllers
{
    /// <summary>
    /// Publicly available methods for getting all hotspots.
    /// </summary>
    [RoutePrefix("hotspots")]
    public class HotspotsController : ApiController
    {
        /// <summary>
        /// Get list of hotspots.
        /// </summary>
        /// <returns>The hotspots.</returns>
        [Route("")]
        [HttpGet]
        [ResponseType(typeof(allHotspots))]
        public HttpResponseMessage getAll()
        {
            Database context = new Database();

            /* For testing purposes.
            Tuple<Bike, GPSLocation>[] locations = context.RunSession(session => session.GetBikeLocations());
            GPSLocation[] locs = locations.Select(x => x.Item2).ToArray(); 
            context.RunSession(session => session.CreateHotspot(locs, false));
            */

            allHotspots hotspotResources = new allHotspots();
            int hotspotsCount = 0;
            foreach (Hotspot item in context.RunSession(session => session.GetAllHotspots()))
            {
                hotspotsCount++;
                hotspot tempHotspot = new hotspot();

                foreach (GPSLocation gpsLoc in item.getDataPoints())
                {
                    coordinate tempCoordinate = new coordinate();
                    tempCoordinate.latitude = gpsLoc.Latitude;
                    tempCoordinate.longtitude = gpsLoc.Longitude;
                    tempHotspot.coordinates.Add(tempCoordinate);
                }

                hotspotResources.hotspots.Add(tempHotspot);

            }
            hotspotResources.count = hotspotsCount;

            return Request.CreateResponse(HttpStatusCode.OK, hotspotResources);
        }
    }
}