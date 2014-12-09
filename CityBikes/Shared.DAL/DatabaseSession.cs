using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DAL
{
    /// <summary>
    /// Represents a single connection to a database, exposing methods for performing queries on the database.
    /// </summary>
    public class DatabaseSession
    {
        private Database database;

        internal DatabaseSession(Database database)
        {
            if (database == null)
                throw new ArgumentNullException("database");

            this.database = database;
        }

        /// <summary>
        /// Executes a query (SELECT).
        /// </summary>
        /// <param name="query">The query that should be executed.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>A <see cref="RowCollection"/> element from which data can be read.</returns>
        internal RowCollection ExecuteRead(string query, params object[] args)
        {
            if (!query.ToLower().Trim().StartsWith("select"))
                throw new ArgumentException("ExecuteRead must be performed with a SELECT query. Use Execute instead.");

            return new RowCollection(string.Format(query, args), database.connection);
        }

        /// <summary>
        /// Executes a query (INSERT, UPDATE, DELETE).
        /// <paramref name="args"/> is a set of arguments for the query applied using <see cref="String.Format"/>.
        /// </summary>
        /// <param name="query">The query that should be executed.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        /// <returns>The number of affected rows.</returns>
        internal int Execute(string query, params object[] args)
        {
            if (query.ToLower().Trim().StartsWith("select"))
                throw new ArgumentException("Execute cannot be performed with a SELECT query. Use ExecuteRead instead.");

            MySqlCommand cmd = new MySqlCommand(string.Format(query, args), database.connection);
            int count = cmd.ExecuteNonQuery();
            cmd.Dispose();

            return count;
        }

        internal int Execute(MySqlCommand command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            command.Connection = database.connection;
            int count = command.ExecuteNonQuery();
            command.Connection = null;

            return count;
        }
    }
}
