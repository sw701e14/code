using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTO;


namespace Shared.DAL
{
    public class SelectQueries
    {

        public GPSData LatestGPSData(Bike b)
        {
            Database db = new Database();
            GPSData gps = db.RunSession<GPSData>(session => session.ExecuteRead("SELECT bikeId, latitude, longitude, accuracy, queried, hasNotMoved FROM citybike_test.gps_data WHERE bikeId = {0} ORDER BY queried DESC", b.Id).First().GetGPSData());
            db.Dispose();
            return gps;
        }

        public byte[] AllMarkovChains(int column)
        {
            Database db = new Database();
            Database.RowCollection serializedMarkovChain = db.RunSession(session=>session.ExecuteRead("SELECT mc FROM markov_chains"));
            byte[] data = serializedMarkovChain.ElementAt(column).GetValue<byte[]>();
            db.Dispose();
            return data;
        }

    }
}
