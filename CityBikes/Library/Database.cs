using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Database : IDisposable
    {
        private MySqlConnection connection;
        private string server;
        private string database;
        private string username;
        private string password;

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
        public void Dispose()
        {
            connection.Dispose();
        }

        public void RunSession(Action<DatabaseSession> operation)
        {
            connection.Open();

            DatabaseSession session = new DatabaseSession(this);
            operation(session);

            connection.Close();
        }

        public class DatabaseSession
        {
            private Database database;

            internal DatabaseSession(Database database)
            {
                if (database == null)
                    throw new ArgumentNullException("database");

                this.database = database;
            }

            public void Execute(string query, params object[] args)
            {
                MySqlCommand cmd = new MySqlCommand(string.Format(query, args), database.connection);
                cmd.ExecuteNonQuery();
                cmd.Dispose();
            }
        }

        public class RowCollection : IEnumerable<Row>
        {
            private readonly string command;
            private readonly MySqlConnection connection;

            public RowCollection(string command, MySqlConnection connection)
            {
                this.command = command;
                this.connection = connection;
            }

            IEnumerator<Row> IEnumerable<Row>.GetEnumerator()
            {
                return new Enumerator(new MySqlCommand(command, connection));
            }
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return new Enumerator(new MySqlCommand(command, connection));
            }

            private class Enumerator : IEnumerator<Row>
            {
                private MySqlCommand command;

                private Row row;
                private MySqlDataReader reader;

                public Enumerator(MySqlCommand command)
                {
                    if (command == null)
                        throw new ArgumentNullException("command");

                    this.command = command;

                    this.row = null;
                    this.reader = null;
                }

                public Row Current
                {
                    get { throw new NotImplementedException(); }
                }

                public void Dispose()
                {
                    throw new NotImplementedException();
                }

                object System.Collections.IEnumerator.Current
                {
                    get { throw new NotImplementedException(); }
                }

                public bool MoveNext()
                {
                    throw new NotImplementedException();
                }

                public void Reset()
                {
                    throw new NotImplementedException();
                }
            }
        }

        public class Row
        {
            private MySqlDataReader reader;

            public Row(MySqlDataReader reader)
            {
                this.reader = reader;
            }

            public T GetValue<T>(int ordinal)
            {
                return reader.GetFieldValue<T>(ordinal);
            }
            public T GetValue<T>(string name)
            {
                return GetValue<T>(reader.GetOrdinal(name));
            }
        }
    }
}
