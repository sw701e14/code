using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Library
{
    /// <summary>
    /// Exposes methods and classes enabling database interactions.
    /// </summary>
    public class Database : IDisposable
    {
        private MySqlConnection connection;
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
                operation(session);
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
                result = operation(session);
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
            public RowCollection ExecuteRead(string query, params object[] args)
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
            public int Execute(string query, params object[] args)
            {
                if (query.ToLower().Trim().StartsWith("select"))
                    throw new ArgumentException("Execute cannot be performed with a SELECT query. Use ExecuteRead instead.");

                MySqlCommand cmd = new MySqlCommand(string.Format(query, args), database.connection);
                int count = cmd.ExecuteNonQuery();
                cmd.Dispose();

                return count;
            }

            public Hotspot CreateHotspot(GPSLocation[] data, bool applyConvexHull)
            {
                if (applyConvexHull)
                    data = GPSLocation.GetConvexHull(data);

                MySqlCommand cmd = new MySqlCommand("INSERT INTO hotspots (convex_hull) VALUES(@data)");
                Hotspot hotspot = new Hotspot(data);

                BinaryFormatter formatter = new BinaryFormatter();
                using (MemoryStream ms = new MemoryStream())
                {
                    formatter.Serialize(ms, data);
                    cmd.Parameters.Add("@data", MySqlDbType.Blob).Value = ms.ToArray();
                }

                Execute(cmd);

                return hotspot;
            }
            public Hotspot[] GetAllHotspots()
            {
                return ExecuteRead("SELECT convex_hull FROM hotspots").Select(row => row.GetHotspot()).ToArray();
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

        /// <summary>
        /// Exposes methods for retrieving data from a single database row.
        /// </summary>
        public class Row
        {
            private object[] data;
            private int tupleIndexShift = 0;

            internal Row(MySqlDataReader reader)
            {
                this.data = new object[reader.FieldCount];
                for (int i = 0; i < data.Length; i++)
                    data[i] = reader.GetValue(i);
            }

            /// <summary>
            /// Gets the value of the specified column as type <typeparamref name="T"/>.
            /// </summary>
            /// <typeparam name="T">The type of the element that is retrieved from the database.</typeparam>
            /// <param name="column">The zero-based column index where data-consumption should start.</param>
            /// <returns>The data at <paramref name="column"/> as type <typeparamref name="T"/>.</returns>
            public T GetValue<T>(int column = 0)
            {
                if (typeof(T) == typeof(Bike))
                    return (T)(object)GetBike(column);

                else if (typeof(T) == typeof(GPSLocation))
                    return (T)(object)GetGPSLocation(column);

                else if (typeof(T) == typeof(GPSData))
                    return (T)(object)GetGPSData(column);

                else if (typeof(T) == typeof(Hotspot))
                    return (T)(object)GetHotspot(column);

                else
                {
                    object item = data[column + tupleIndexShift];

                    if (item is DBNull)
                        return default(T);

                    return (T)item;
                }
            }

            public Bike GetBike(int column = 0)
            {
                object item = data[column + tupleIndexShift];
                return new Bike((uint)item);
            }

            public GPSLocation GetGPSLocation(int column = 0)
            {
                object item1 = data[column + tupleIndexShift];
                object item2 = data[column + tupleIndexShift + 1];

                tupleIndexShift++;

                return new GPSLocation((decimal)item1, (decimal)item2);
            }

            public GPSData GetGPSData(int column = 0)
            {
                Bike bike = GetBike(column);
                GPSLocation location = GetGPSLocation(column + 1);
                byte acc = GetValue<byte>(column + 2);
                DateTime query = GetValue<DateTime>(column + 3);
                bool notMoved = GetValue<bool>(column + 4);

                tupleIndexShift += 4;
                return new GPSData(bike, location, acc, query, notMoved);
            }

            public Hotspot GetHotspot(int column = 0)
            {
                object item = data[column + tupleIndexShift];

                MemoryStream stream = new MemoryStream((byte[])item);
                BinaryFormatter formatter = new BinaryFormatter();

                GPSLocation[] points = (GPSLocation[])formatter.Deserialize(stream);

                stream.Close();
                stream.Dispose();

                return new Hotspot(points);
            }

            /// <summary>
            /// Converts the row into a <see cref="Tuple"/> where each element corresponds to a column.
            /// </summary>
            /// <typeparam name="T1">The type of the first column.</typeparam>
            /// <param name="column">The zero-based column index where data-consumption should start.</param>
            /// <returns>A <see cref="Tuple"/> whose value is the data from the row.</returns>
            public Tuple<T1> ToTuple<T1>(int column = 0)
            {
                tupleIndexShift = column;
                return Tuple.Create(GetValue<T1>(0));
            }
            /// <summary>
            /// Converts the row into a <see cref="Tuple"/> where each element corresponds to a column.
            /// </summary>
            /// <typeparam name="T1">The type of the first column.</typeparam>
            /// <typeparam name="T2">The type of the second column.</typeparam>
            /// <param name="column">The zero-based column index where data-consumption should start.</param>
            /// <returns>A <see cref="Tuple"/> whose value is the data from the row.</returns>
            public Tuple<T1, T2> ToTuple<T1, T2>(int column = 0)
            {
                tupleIndexShift = column;
                return Tuple.Create(GetValue<T1>(0), GetValue<T2>(1));
            }
            /// <summary>
            /// Converts the row into a <see cref="Tuple"/> where each element corresponds to a column.
            /// </summary>
            /// <typeparam name="T1">The type of the first column.</typeparam>
            /// <typeparam name="T2">The type of the second column.</typeparam>
            /// <typeparam name="T3">The type of the third column.</typeparam>
            /// <param name="column">The zero-based column index where data-consumption should start.</param>
            /// <returns>A <see cref="Tuple"/> whose value is the data from the row.</returns>
            public Tuple<T1, T2, T3> ToTuple<T1, T2, T3>(int column = 0)
            {
                tupleIndexShift = column;
                return Tuple.Create(GetValue<T1>(0), GetValue<T2>(1), GetValue<T3>(2));
            }
            /// <summary>
            /// Converts the row into a <see cref="Tuple"/> where each element corresponds to a column.
            /// </summary>
            /// <typeparam name="T1">The type of the first column.</typeparam>
            /// <typeparam name="T2">The type of the second column.</typeparam>
            /// <typeparam name="T3">The type of the third column.</typeparam>
            /// <typeparam name="T4">The type of the fourth column.</typeparam>
            /// <param name="column">The zero-based column index where data-consumption should start.</param>
            /// <returns>A <see cref="Tuple"/> whose value is the data from the row.</returns>
            public Tuple<T1, T2, T3, T4> ToTuple<T1, T2, T3, T4>(int column = 0)
            {
                tupleIndexShift = column;
                return Tuple.Create(GetValue<T1>(0), GetValue<T2>(1), GetValue<T3>(2), GetValue<T4>(3));
            }
            /// <summary>
            /// Converts the row into a <see cref="Tuple"/> where each element corresponds to a column.
            /// </summary>
            /// <typeparam name="T1">The type of the first column.</typeparam>
            /// <typeparam name="T2">The type of the second column.</typeparam>
            /// <typeparam name="T3">The type of the third column.</typeparam>
            /// <typeparam name="T4">The type of the fourth column.</typeparam>
            /// <typeparam name="T5">The type of the fifth column.</typeparam>
            /// <param name="column">The zero-based column index where data-consumption should start.</param>
            /// <returns>A <see cref="Tuple"/> whose value is the data from the row.</returns>
            public Tuple<T1, T2, T3, T4, T5> ToTuple<T1, T2, T3, T4, T5>(int column = 0)
            {
                tupleIndexShift = column;
                return Tuple.Create(GetValue<T1>(0), GetValue<T2>(1), GetValue<T3>(2), GetValue<T4>(3), GetValue<T5>(4));
            }
            /// <summary>
            /// Converts the row into a <see cref="Tuple"/> where each element corresponds to a column.
            /// </summary>
            /// <typeparam name="T1">The type of the first column.</typeparam>
            /// <typeparam name="T2">The type of the second column.</typeparam>
            /// <typeparam name="T3">The type of the third column.</typeparam>
            /// <typeparam name="T4">The type of the fourth column.</typeparam>
            /// <typeparam name="T5">The type of the fifth column.</typeparam>
            /// <typeparam name="T6">The type of the sixth column.</typeparam>
            /// <param name="column">The zero-based column index where data-consumption should start.</param>
            /// <returns>A <see cref="Tuple"/> whose value is the data from the row.</returns>
            public Tuple<T1, T2, T3, T4, T5, T6> ToTuple<T1, T2, T3, T4, T5, T6>(int column = 0)
            {
                tupleIndexShift = column;
                return Tuple.Create(GetValue<T1>(0), GetValue<T2>(1), GetValue<T3>(2), GetValue<T4>(3), GetValue<T5>(4), GetValue<T6>(5));
            }
            /// <summary>
            /// Converts the row into a <see cref="Tuple"/> where each element corresponds to a column.
            /// </summary>
            /// <typeparam name="T1">The type of the first column.</typeparam>
            /// <typeparam name="T2">The type of the second column.</typeparam>
            /// <typeparam name="T3">The type of the third column.</typeparam>
            /// <typeparam name="T4">The type of the fourth column.</typeparam>
            /// <typeparam name="T5">The type of the fifth column.</typeparam>
            /// <typeparam name="T6">The type of the sixth column.</typeparam>
            /// <typeparam name="T7">The type of the seventh column.</typeparam>
            /// <param name="column">The zero-based column index where data-consumption should start.</param>
            /// <returns>A <see cref="Tuple"/> whose value is the data from the row.</returns>
            public Tuple<T1, T2, T3, T4, T5, T6, T7> ToTuple<T1, T2, T3, T4, T5, T6, T7>(int column = 0)
            {
                tupleIndexShift = column;
                return Tuple.Create(GetValue<T1>(0), GetValue<T2>(1), GetValue<T3>(2), GetValue<T4>(3), GetValue<T5>(4), GetValue<T6>(5), GetValue<T7>(6));
            }

            /// <summary>
            /// Gets the data type of the specified column.
            /// </summary>
            /// <param name="column">The zero-based column index.</param>
            /// <returns>The data type of the specified column</returns>
            public Type GetType(int column)
            {
                return data[column].GetType();
            }
        }
    }
}
