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
        double[,] markovChain;
        private double[,] markovFrequencies;

        public static Matrix BuildMarkovMatrix(Hotspot[] hotspots, GPSData[] data)
        {
            var first = data.Min(d => d.QueryTime);
            var step = TimeSpan.FromMinutes(5);
            var groups = groupData(data).ToArray();

            int size = hotspots.Length * 2;
            int[,] counter = new int[size, size];
            int[] last = new int[groups.Length];

            for (int b = 0; b < groups.Length; b++)
            {
                int index = getHotspotIndex(hotspots, groups[b].GetData(first).Location);
                last[b] = index >= 0 ? index * 2 : (~index + 1);
            }

            throw new NotImplementedException();
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

        public double this[int i, int j]
        {
            get { return markovFrequencies[i, j]; }
            set { markovFrequencies[i, j] = value; }
        }

        public MarkovChain(int size)
        {
            markovFrequencies = new double[size, size];
            markovChain = new double[size, size];
        }

        private int Sum()
        {
            int k = 0;
            foreach (var item in markovFrequencies)
            {
                k += (int)item;
            }
            return k;
        }

        public int size
        {
            get { return markovChain.GetLength(0); }
        }



        /// <summary>
        /// Creates the markov chain from the frequencies inputted and saves it in the MarkovChain
        /// </summary>
        public void CreateChain()
        {
            int s = Sum();

            for (int i = 0; i < markovFrequencies.GetLength(0); i++)
            {
                for (int j = 0; j < markovFrequencies.GetLength(1); j++)
                {
                    markovChain[i, j] = markovFrequencies[i, j] / s;
                }
            }
        }
    }
}
