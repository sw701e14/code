using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseImport
{
    public static class DatabaseExport
    {
        private const int INTERVAL = 5;
        /// <summary>
        /// Exports the specified points directly to the database.
        /// </summary>
        /// <param name="context">The database to which the point should be exported.</param>
        /// <param name="points">The points that should be exported.</param>
        public static void Export(Database.DatabaseSession session, IEnumerable<GPSData> points)
        {
            var pArr = points.ToArray();

            List<Bike> oldbikes = session.ExecuteRead("SELECT * FROM citybike_test.bikes").Select(row => row.GetBike()).ToList();
            List<Bike> newbikes = new List<Bike>();

            foreach (var p in pArr)
            {
                if (!oldbikes.Contains(p.Bike))
                {
                    newbikes.Add(p.Bike);
                    oldbikes.Add(p.Bike);
                }

                GPSData movedPoint = DataSources.GPSDataExtension.Randomize(p);
                BikeUpdateLocation.InsertLocation(session, movedPoint);
            }
        }
    }
}
