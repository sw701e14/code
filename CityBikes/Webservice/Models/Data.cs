using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Shared.DAL;
using Shared.DTO;

namespace Webservice.Models
{
    public static class Data
    {
        private const int IMMOBILE_MINUTES = 10;

        /// <summary>
        /// Gets a collection of all the available bikes.
        /// </summary>
        /// <param name="session">A <see cref="DatabaseSession"/> from which data should be retrieved.</param>
        /// <returns>A collection of bikes and their location.</returns>
        public static Tuple<Shared.DTO.Bike, GPSLocation>[] GetAvailableBikes()
        {
            GPSData[] data;
            using (Database db = new Database())
                data = db.RunSession(session => Shared.DTO.Bike.GetLatestData(session));

            var immobileTimeSpan = new TimeSpan(0, IMMOBILE_MINUTES, 0);
            DateTime now = DateTime.Now;

            return (from b in data
                    where (now - b.QueryTime).CompareTo(immobileTimeSpan) > 0
                    select Tuple.Create(b.Bike, b.Location)).ToArray();
        }

        public static IEnumerable<Bike> GetAllBikes()
        {
            GPSData[] data;
            using (Database db = new Database())
                data = db.RunSession(session => Shared.DTO.Bike.GetLatestData(session));

            Array.Sort(data, (x, y) => x.Bike.Id.CompareTo(y.Bike.Id));

            foreach (GPSData item in data)
            {
                Bike b = new Bike();
                b.Id = item.Bike.Id;
                b.Latitude = item.Location.Latitude;
                b.Longitude = item.Location.Longitude;
                b.ImmobileSince = item.QueryTime;

                yield return b;
            }
        }

        public static IEnumerable<Hotspot> GetAllHotspots()
        {
            Shared.DTO.Hotspot[] hotspots;
            using (Database context = new Database())
                hotspots = context.RunSession(s => Shared.DTO.Hotspot.LoadAllHotspots(s));

            foreach (var hs in hotspots)
                yield return Hotspot.ConvertFromHotspot(hs);
        }

        public static IEnumerable<Prediction> GetPredictions()
        {
            MarkovChain markov;
            Vector init;
            using (Database db = new Database())
            {
                markov = db.RunSession(session => MarkovChain.LoadMarkovChain(session));
                init = db.RunSession(session => markov.BuildInitialState(session));
            }

            for (int i = 0; i < markov.Hotspots.Length; i++)
            {
                var hs = markov.Hotspots[i];
                var m2 = markov.CloneWithWaitState(hs);

                var probs = init;
                TimeSpan time = TimeSpan.Zero;

                while (probs[i * 2] < 1)
                {
                    probs *= markov.Probabilities;
                    time = time.Add(MarkovChain.TIME_INTERVAL);
                }

                yield return new Prediction(Hotspot.ConvertFromHotspot(hs), time);
            }
        }
    }
}