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
        /// <summary>
        /// Exports the specified points directly to the database.
        /// </summary>
        /// <param name="database">The database to which the point should be exported.</param>
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
            }
            session.Execute("INSERT INTO bikes (id) VALUES{0}", string.Join(", ", (from b in newbikes select "(" + b.Id + ")").ToArray()));
            session.Execute("INSERT INTO gps_data (bikeId, latitude, longitude, accuracy, queried, hasNotMoved) VALUES{0}",
                string.Join(", ", (from p in pArr select formatGPS(p)).ToArray()));
        }

        private static string formatGPS(GPSData data)
        {
            return string.Format("({0}, {1}, {2}, {3}, {4}, {5})",
                data.Bike.Id,
                data.Location.Latitude,
                data.Location.Longitude,
                data.Accuracy,
                data.QueryTime.ToString("yyyy-MM-dd hh:mm:ss"),
                data.HasNotMoved);
        }
    }
}
