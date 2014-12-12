using Shared.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTO
{
    public class MarkovChain
    {
        private Matrix probabilities;
        private Hotspot[] hotspots;

        public static void CreateMarkovChain(DatabaseSession session, Hotspot[] hotspots, GPSData[] data)
        {
            Matrix matrix = buildMarkovMatrix(hotspots, data);

            session.InsertMarkovMatrix(matrix.ToArray(), hotspots.Select(hs => hs.id).ToArray());
        }

        public static MarkovChain LoadMarkovChain(DatabaseSession session)
        {
            var data = session.GetMarkovChain();

            Matrix matrix = new Matrix(data.Item1);
            Hotspot[] hotspots = data.Item2.Select(id => Hotspot.LoadHotspot(session, id)).ToArray();

            return new MarkovChain(hotspots, matrix);
        }

        #region Methods and types related to building markov matrices

        private static Matrix buildMarkovMatrix(Hotspot[] hotspots, GPSData[] data)
        {
            var start = data.Min(d => d.QueryTime);
            var end = data.Max(d => d.QueryTime);

            var step = TimeSpan.FromMinutes(5);
            var groups = groupData(data).ToArray();

            int size = hotspots.Length * 2;
            double[,] counter = new double[size, size];
            int[] last = new int[groups.Length];

            for (int b = 0; b < groups.Length; b++)
                last[b] = getHotspotIndex(hotspots, groups[b].GetData(start).Location);

            for (DateTime d = start; d <= end; d = d.Add(step))
            {
                for (int b = 0; b < groups.Length; b++)
                {
                    int old = last[b];
                    int next = getHotspotIndex(hotspots, groups[b].GetData(d).Location);

                    if (next % 2 == 1)
                    {
                        next = old;
                        if (old % 2 == 0)
                            next++;
                    }

                    counter[last[b], next]++;

                    last[b] = next;
                }
            }

            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                for (int j = 0; j < size; j++) sum += counter[i, j];
                if (sum > 0)
                    for (int j = 0; j < size; j++) counter[i, j] /= sum;
            }
            return new Matrix(counter);
        }

        private static IEnumerable<dataenumerator> groupData(GPSData[] data)
        {
            Array.Sort(data, buildSort);

            List<GPSData> list = new List<GPSData>();

            Bike current = data[0].Bike;
            list.Add(data[0]);
            for (int i = 1; i < data.Length; i++)
            {
                if (current != data[i].Bike)
                {
                    yield return new dataenumerator(list.ToArray());
                    list.Clear();
                    current = data[i].Bike;
                }
                list.Add(data[i]);
            }
            if (list.Count > 0)
                yield return new dataenumerator(list.ToArray());
        }
        private static int buildSort(GPSData d1, GPSData d2)
        {
            int diff = d1.Bike.Id.CompareTo(d2.Bike.Id);
            if (diff == 0)
                diff = d1.QueryTime.CompareTo(d2.QueryTime);
            return diff;
        }

        private static int getHotspotIndex(Hotspot[] hotspots, GPSLocation location)
        {
            double dist = double.PositiveInfinity;
            int index = int.MaxValue;

            for (int i = 0; i < hotspots.Length; i++)
            {
                double d = hotspots[i].DistanceTo(location);
                if (d < dist)
                {
                    dist = d;
                    index = i;
                }
            }

            return hotspots[index].Contains(location) ? index * 2 : (index * 2 + 1);
        }

        private class dataenumerator
        {
            private GPSData[] data;

            public dataenumerator(GPSData[] data)
            {
                this.data = data;
            }

            public GPSData GetData(DateTime time)
            {
                if (time < data[0].QueryTime)
                    return data[0];

                for (int i = 1; i < data.Length; i++)
                    if (time < data[i].QueryTime)
                        return data[i - 1];

                return data[data.Length - 1];
            }
        }

        #endregion

        private MarkovChain(Hotspot[] hotspots, Matrix matrix)
        {
            this.hotspots = hotspots;
            this.probabilities = matrix;
        }

        public Matrix Probabilities
        {
            get { return probabilities; }
        }
    }
}
