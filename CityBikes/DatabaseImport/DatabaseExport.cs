using Library.GeneratedDatabaseModel;
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
        public static void Export(Database database, IEnumerable<gps_data> points)
        {
            foreach (var p in points)
                database.gps_data.AddObject(p);
        }
    }
}
