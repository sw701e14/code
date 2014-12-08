using Shared.DAL;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelUpdater
{
    public class MarkovChain
    {
        private Matrix probabilities;

        public static Matrix BuildMarkovMatrix(Hotspot[] hotspots, GPSData[] data)
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

            for (DateTime d = start; d <= end; d.Add(step))
            {
                for (int b = 0; b < groups.Length; b++)
                {
                    int next = getHotspotIndex(hotspots, groups[b].GetData(d).Location);

                    if (next % 2 == 1 && last[b] % 2 == 1)
                        next = last[b];
                    else if (next % 2 == 1 && last[b] % 2 == 0)
                        next = last[b] + 1;

                    counter[last[b], next]++;

                    last[b] = next;
                }
            }

            for (int i = 0; i < size; i++)
            {
                double sum = 0;
                for (int j = 0; j < size; j++) sum += counter[i, j];
                for (int j = 0; j < size; j++) counter[i, j] /= sum;
            }
            return new Matrix(counter);
        }

        #region Methods and types related to building markov matrices

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

        public MarkovChain(Matrix matrix)
        {
            this.probabilities = matrix;
        }

        public Matrix Probabilities
        {
            get { return probabilities; }
        }
    }
}
