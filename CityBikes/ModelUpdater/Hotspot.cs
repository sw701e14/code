using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Shared.DAL;
using Shared.DTO;

namespace ModelUpdater
{
    class Hotspot
    {
        public static void CreateHotspot(GPSLocation[] data, bool applyConvexHull)
        {
            if (applyConvexHull)
                data = GPSLocation.GetConvexHull(data);

            using (Database db = new Database())
            {
                db.RunSession(session=>session.InsertHotSpot(data));
            }
        }

    }
}
