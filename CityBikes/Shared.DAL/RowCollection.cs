using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Shared.DAL
{
    /// <summary>
    /// Represents a collection of <see cref="Row"/> instances returned from a database query.
    /// </summary>
    public class RowCollection : IEnumerable<Row>
    {
        private readonly string command;
        private readonly MySqlConnection connection;

        internal RowCollection(string command, MySqlConnection connection)
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
                get
                {
                    if (row == null)
                        row = new Row(reader);
                    return row;
                }
            }

            public void Dispose()
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                command.Dispose();
            }

            object System.Collections.IEnumerator.Current
            {
                get { return row; }
            }

            public bool MoveNext()
            {
                if (reader == null)
                    reader = command.ExecuteReader();

                row = null;

                return reader.Read();
            }

            public void Reset()
            {
                // Reset is defined by the IEnumerator interface, but is not used by the .NET framework.
                // See http://msdn.microsoft.com/en-us/library/system.collections.ienumerator.reset%28v=vs.110%29.aspx for details.
                throw new NotSupportedException();
            }
        }
    }
}
