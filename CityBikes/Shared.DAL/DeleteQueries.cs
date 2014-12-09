using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DAL
{
    public static class DeleteQueries
    {
        public static void TruncateAll(this DatabaseSession session)
        {
            session.Execute("TRUNCATE citybike_test.gps_data; TRUNCATE citybike_test.bikes; TRUNCATE citybike_test.hotspots; TRUNCATE citybike_test.markov_chains");
        }

        public static void TruncateGPS_data(this DatabaseSession session)
        {
             session.Execute("TRUNCATE TABLE gps_data");
        }

        public static void TruncateMarkov_chains(this DatabaseSession session)
        {
            session.Execute("TRUNCATE citybike_test.markov_chains");
        }
    }
}
