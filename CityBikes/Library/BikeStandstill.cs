using Library.GeneratedDatabaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public static class BikeStandstill
    {
        private static Database database = new Database();

        public static IEnumerable<Tuple<int, DateTime>> GetBikesImmobile()
        {

        }
        public static IEnumerable<Tuple<int, DateTime>> GetBikesImmobile(DateTime immobileSince)
        {
            return GetBikesImmobile().Where(b => b.Item2 < immobileSince);
        }
    }
}
