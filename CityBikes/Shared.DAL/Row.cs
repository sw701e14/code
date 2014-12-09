using MySql.Data.MySqlClient;
using Shared.DTO;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Shared.DAL
{
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

        public Matrix GetMatrix(int column = 0)
        {
            object item = data[column + tupleIndexShift];

            MemoryStream stream = new MemoryStream((byte[])item);
            BinaryReader reader = new BinaryReader(stream);

            int w = reader.ReadInt32();
            int h = reader.ReadInt32();

            double[,] m = new double[w, h];
            for (int y = 0; y < h; y++)
                for (int x = 0; x < w; x++)
                    m[x, y] = reader.ReadDouble();

            reader.Close();
            reader.Dispose();

            return new Matrix(m);
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
