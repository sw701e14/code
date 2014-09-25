using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.GeneratedDatabaseModel;

namespace Library
{
    public class AllBikesLocation
    {
        Database context = new Database();
        public gps_data GetBikeLocation(int Id)
        {
            var location = (from bike in context.gps_data where bike.bikeId == Id orderby bike.queried descending select bike);

            return location.First();
        }

        public IQueryable<gps_data> GetBikeLocations()
        {
           var bg =  from bike in context.gps_data 
                           group bike by bike.bikeId into b
                           select b.OrderByDescending(x=>x.queried).First();
           var h = bg.ToArray();
           return bg;
        }

        public IEnumerable<int> findAllIds()
        {
            return from bike in context.gps_data group bike by bike.bikeId into b select b.Key;
        }

        public gps_data FindLocationsOfAllBikes(IQueryable bikeIds)
        {
            var bikes = findAllIds();

            return new gps_data();
        }
    }
}
