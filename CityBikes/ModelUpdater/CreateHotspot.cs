using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Shared.DTO;

namespace ModelUpdater
{
    class CreateHotspot
    {
        public static void CreateHotspot(GPSLocation[] data, bool applyConvexHull)
        {
            if (applyConvexHull)
                data = GPSLocation.GetConvexHull(data);

            Shared.DAL.InsertQueries.InsertHotSpot(data);
        }

    }
}
