using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTO;

namespace Shared.DAL
{
    public static class UpdateQueries
    {
        public static void setHasNotMoved(this DatabaseSession session, Bike bike)
        {
             session.Execute(
@"UPDATE gps_data a
INNER JOIN
(
    SELECT  bikeId, MAX(id) id
    FROM    gps_data
    GROUP   BY bikeId
) b ON  a.id = b.id AND {0} = b.bikeId
SET a.hasNotMoved = '1'", bike.Id);


        }
    }
}
