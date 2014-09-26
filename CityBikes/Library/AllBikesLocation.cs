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

        public IEnumerable<Tuple<int, GPSLocation>> GetBikeLocations()
        {
            return from bike in context.gps_data
                     group bike by bike.bikeId into b
                     let newestLocation = b.Max(x => x.queried)

                     from g in b
                     where g.queried == newestLocation
                     select  Tuple.Create(g.bikeId, g.Location);

            
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
