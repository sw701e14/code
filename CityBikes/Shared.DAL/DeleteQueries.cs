using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DAL
{
    public static class DeleteQueries
    {
        /// <summary>
        /// Truncates all tables in the database.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> using which data should be deleted.</param>
        public static void TruncateAll(this DatabaseSession session)
        {
            session.Execute("TRUNCATE citybike_test.gps_data; TRUNCATE citybike_test.bikes; TRUNCATE citybike_test.hotspots; TRUNCATE citybike_test.markov_chains");
        }

        /// <summary>
        /// Truncates the table with GPS data.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> using which data should be deleted.</param>
        public static void TruncateGPS_data(this DatabaseSession session)
        {
             session.Execute("TRUNCATE TABLE gps_data");
        }

        /// <summary>
        /// Truncates the table with markov chains.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> using which data should be deleted.</param>
        public static void TruncateMarkov_chains(this DatabaseSession session)
        {
            session.Execute("TRUNCATE citybike_test.markov_chains");
        }

        /// <summary>
        /// Truncates the table with hotspots.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> using which data should be deleted.</param>
        public static void TruncateHotspots(this DatabaseSession session)
        {
            session.Execute("TRUNCATE citybike_test.hotspots");
        }
    }
}
