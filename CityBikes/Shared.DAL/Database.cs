using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using Shared.DTO;

namespace Shared.DAL
{
    /// <summary>
    /// Exposes methods and classes enabling database interactions.
    /// </summary>
    public class Database : IDisposable
    {
        internal MySqlConnection connection;
        private string server;
        private string database;
        private string username;
        private string password;

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        public Database()
        {
            Initialize();
        }
        private void Initialize()
        {
            server = "localhost";
            database = "citybike_test";
            username = "root";
            password = "takecare";
            string connectionString;
            connectionString = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + username + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            connection.Dispose();
        }


        /// <summary>
        /// Runs a database session by connection to the database, executing <paramref name="operation"/> and disconnecting.
        /// </summary>
        /// <param name="operation">The operation that should be performed on the database.</param>
        public void RunSession(Action<DatabaseSession> operation)
        {
            connection.Open();

            DatabaseSession session = new DatabaseSession(this);
            Exception error = null;

            try
            {
                var trans = connection.BeginTransaction();
                operation(session);
                trans.Commit();
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                connection.Close();
            }

            if (error != null)
                throw error;
        }

        /// <summary>
        /// Runs a database session and retrieves a result.
        /// </summary>
        /// <typeparam name="T">The type of the result returned by <paramref name="operation"/>.</typeparam>
        /// <param name="operation">The operation that should be performed on the database.</param>
        /// <returns>The result of <paramref name="operation"/>.</returns>
        public T RunSession<T>(Func<DatabaseSession, T> operation)
        {
            connection.Open();

            DatabaseSession session = new DatabaseSession(this);
            Exception error = null;

            T result = default(T);

            try
            {
                var trans = connection.BeginTransaction();
                result = operation(session);
                trans.Commit();
            }
            catch (Exception ex)
            {
                error = ex;
            }
            finally
            {
                connection.Close();
            }

            if (error != null)
                throw error;
            else
                return result;
        }
    }
}
