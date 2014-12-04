using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Shared.DTO;


namespace Shared.DAL
{
    public class InsertQueries
    {
        public static void InsertMarkovChain(Database.DatabaseSession session, MarkovChain markovChain)
        {
            MySqlCommand cmd = new MySqlCommand("INSERT INTO markov_chains (mc) VALUES(@data)");
            cmd.Parameters.Add("@data", MySqlDbType.MediumBlob).Value = serializeMarkovChain(markovChain);

            session.Execute(cmd);
        }
    }
}
