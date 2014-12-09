using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DAL
{
    public static class UpdateQueries
    {
        /// <summary>
        /// Sets the hasNotMoved field of a bike to true.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> in which data should be updated.</param>
        /// <param name="bikeId">The bike identifier.</param>
        public static void SetHasNotMoved(this DatabaseSession session, uint bikeId)
        {
             session.Execute(
@"UPDATE gps_data a
INNER JOIN
(
    SELECT  bikeId, MAX(id) id
    FROM    gps_data
    GROUP   BY bikeId
) b ON  a.id = b.id AND {0} = b.bikeId
SET a.hasNotMoved = '1'", bikeId);
        }
    }
}
