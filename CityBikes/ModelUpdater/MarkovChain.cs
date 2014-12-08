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
            var dict = groupData(data);
        }
        private static Dictionary<Bike, GPSData[]> groupData(GPSData[] data)
        {
            Array.Sort(data, buildSort);

            Dictionary<Bike, GPSData[]> dict = new Dictionary<Bike, GPSData[]>();
            List<GPSData> list = new List<GPSData>();

            Bike current = data[0].Bike;
            list.Add(data[0]);
            for (int i = 1; i < data.Length; i++)
            {
                if (current != data[i].Bike)
                {
                    dict.Add(current, list.ToArray());
                    list.Clear();
                    current = data[i].Bike;
                }
                list.Add(data[i]);
            }
            if (list.Count > 0) dict.Add(current, list.ToArray());

            return dict;
        }
        private static int buildSort(GPSData d1, GPSData d2)
        {
            int diff = d1.Bike.Id.CompareTo(d2.Bike.Id);
            if (diff == 0)
                diff = d1.QueryTime.CompareTo(d2.QueryTime);
            return diff;
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
